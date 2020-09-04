using Gateway.Payload.DataObjects;
using Gateway.Payload.DataObjects.Dispatch.DispatchEvents;
using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway
{
    internal class Gateway
    {
        internal int SessionNumber { get; private set; } = default;
        #region Delegates and events
        private delegate void socketCloseReceived(WebSocketCloseStatus? status, string description);
        private event socketCloseReceived SocketCloseReceived = delegate { };

        internal delegate void PayloadReceived(string payloadJson);
        internal delegate void reachedRateLimit(TimeSpan tillNextMinute);
        internal delegate void VoidEvent();
        internal event PayloadReceived NewPayloadReceived = delegate { };
        internal event VoidEvent Zombied = delegate { };
        internal event reachedRateLimit ReachedRateLimit = delegate { };
        #endregion
        #region Private fields
        private readonly Task socketListener,
                                rateLimitListener;
        private readonly Task heart;
        private Uri gatewayUri;
        private ClientWebSocket socket;
        private readonly SocketLocker socketHelper;
        private TimeSpan heartbeatRate;
        private int payloadSentLastMinute = 0;
        private bool heartbeatAckReceived = true;
        private string sessionIdentifier,
                       botToken;
        private int chunkSize = 1024 * 4, //TODO : подтягивать из конфига, 
                    messageCount = 0,
                    messageTotalLength = 0,
                    lastSequence = 0;
        #endregion
        #region Thread's tasks
        private async void ListenToRateLimit()
        {
            int oneMinute = 60000; //From ms 
            int limit = 120;
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Start();
                while (stopwatch.ElapsedMilliseconds < oneMinute)
                {
                    // Здесь может возникнуть вопрос "А почему мы используем ограничение равное 118?"
                    // Ответом на этот вопрос будет то, что ограничение отправляемых к Gateway дискорда 
                    // Равняется 120 запросов\минута. Но некоторые запросы, такие как Hearbeat 
                    // Необходимо отправлять каждые, в данный момент времени, 41.5 секунд.
                    // Для решения это проблемы я ввёл ограничение на (limit - 2) запроса, дабы
                    // 2 запроса остались в запасе для приоритетных запросов.
                    if (payloadSentLastMinute == limit - 2) //TODO : вынести хардлок в конфиг?
                    {
                        using (SocketSendLockToken token = socketHelper.GetSendingLock())
                        {
                            while (stopwatch.ElapsedMilliseconds < oneMinute)
                            {
                                if(payloadSentLastMinute == limit)
                                {
                                    token.HardLockSocket();
                                    TimeSpan msToWait = TimeSpan.FromMilliseconds(oneMinute - stopwatch.ElapsedMilliseconds); //Через этот отрезок времени наступит новая минута => можно сбрасывать счётчик;
                                    ReachedRateLimit(msToWait);
                                    await Task.Delay(msToWait);
                                    break;
                                }
                            }
                        }
                    }
                    //TODO : здесь всё же может быть проблема с тем, что количество пакетов будет == 120, даже с HighPriority
                }
                Interlocked.Exchange(ref payloadSentLastMinute, 0);
                stopwatch.Restart();
            }
        }
        private async void ListenToSocket()
        {
            byte[] buffer = new byte[chunkSize];
            int capacity = 256; // TODO : изначальный размер буффера
            StringBuilder jsonResultBuilder = new StringBuilder(capacity);
            WebSocketReceiveResult result = null;
            while (true) //TAI : проверить разность быстродействия при обнулении буффера и без обнуления
            {
                //buffer = new byte[chunkSize]; //TAI : нужно ли здесь обнуление?
                jsonResultBuilder.Clear();
                jsonResultBuilder.Capacity = CalculateJsonBuilderCapacity(jsonResultBuilder.Length);
                try
                {
                    using (SocketAccessToken token = socketHelper.GetReceiveAccess())
                    {
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token.CancellationToken)
                                             .ConfigureAwait(false);
                        jsonResultBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        while (!result.EndOfMessage)
                        {
                            //buffer = new byte[chunkSize]; //TAI : нужно ли здесь обнуление?
                            result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token.CancellationToken)
                                                 .ConfigureAwait(false);
                            jsonResultBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        }
                    }
                }
                catch(OperationCanceledException ex) 
                {
                    // TODO : инструмент логирования
                    continue;
                }
                catch(WebSocketException ex)
                {
                    // TODO : инструмент логирования
                    Reconnect(); // TODO: здесь отправляется двойной резюм
                    continue;
                }
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    SocketCloseReceived(result.CloseStatus, result.CloseStatusDescription); // TODO : эта херня работает не так как нужно
                }
                else
                {
                    #pragma warning disable 4014 // Fire and forget
                    string payload = jsonResultBuilder.ToString();
                    Task.Factory.StartNew(() => NewPayloadReceived(payload))
                                .ConfigureAwait(false);
                }
            }
        }
        private async void Heartbeat()
        {
            Heartbeat heartbeatObj = new Heartbeat();
            GatewayPayload payload = new GatewayPayload(Opcode.Heartbeat, heartbeatObj);
            while (true)
            {
                if (heartbeatAckReceived)
                {
                    heartbeatObj.Sequence =  lastSequence;
                    heartbeatAckReceived = false;
                    await SendAsync(payload, true, WebSocketMessageType.Text, CancellationToken.None);                    
                }
                else
                {
                    Zombied();
                }

                await Task.Delay(heartbeatRate);
            }
        }
        #endregion
        #region Event handlers
        private void OnLimitReached(TimeSpan tillNextMinute)
        {
            Console.WriteLine("PayloadSentLimitReached, last till next minute: {0}", tillNextMinute);
        }
        internal void OnConnection(IGatewayDataObject data)
        {
            Hello hello = data as Hello;
            heartbeatRate = hello.HeartbeatInterval;
            heart.Start();
        }
        internal void OnHeartbeatAck(IGatewayDataObject data) => heartbeatAckReceived = true;
        internal void OnSequenceReceived(int sequence) 
        {
            if(sequence > lastSequence)
            {
                lastSequence = sequence;
            }
        }
        internal void OnReady(object sender, EventHandlerArgs args)
        {
            Ready ready = args.EventData as Ready;
            sessionIdentifier = ready.SessionIdentifier;
            if(++SessionNumber != 1)
            {
                Interlocked.Exchange(ref lastSequence, 0);
            }
        }
        internal void OnReconnectRequested(IGatewayDataObject payload)
        {
            Reconnect();
        }
        internal void OnInvalidSessionReceived(IGatewayDataObject payload)
        {
            
        }
        #endregion
        #region Private method's
        private async void Reconnect()
        {
            using (socketHelper.GetSuspendingLock(true))
            {
                socket.Abort();
                socket = null;
                socket = new ClientWebSocket();
                await socket.ConnectAsync(gatewayUri, CancellationToken.None);
            }
            await SendResume();
        }
        private int CalculateJsonBuilderCapacity(int builderLenth) //TODO : Считать медиану
        {
            messageCount++;
            messageTotalLength += builderLenth;
            return 200;
        }
        private async Task SendResume()
        {
            Resume resumeObj = new Resume(botToken, sessionIdentifier, lastSequence); 
            GatewayPayload payload = new GatewayPayload(Opcode.Resume, resumeObj);
            await SendAsync(payload, true, WebSocketMessageType.Text, CancellationToken.None);
        }
        #endregion
        #region Internal method's
        internal async Task ConnectToGatewayAsync()
        {
            await socket.ConnectAsync(gatewayUri, CancellationToken.None);
            if (socket.State != WebSocketState.Open)
            {
                // TODO : инструмент логирования ("Cannot connect to gateway. Retrying in 5s");
                await Task.Delay(5000);
                await ConnectToGatewayAsync();
                return;
            }
            rateLimitListener.Start();
            socketListener.Start();
        }
        internal async Task SendAsync(GatewayPayload payload, 
                                      bool highPriority, 
                                      WebSocketMessageType msgType, 
                                      CancellationToken cts) // TODO : заменить CTS на Action
        {
            Console.WriteLine($"Sending: {payload.Opcode}");
            payload.Sequence = lastSequence;
            string jsonPayload = JsonConvert.SerializeObject(payload, typeof(GatewayPayload), null); // TODO : сериализацию с помощью внещнего проекта
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);

            byte chunksCount = 0;
            for (int dataLength = jsonBytes.Length; dataLength > 0; dataLength -= chunkSize)
                chunksCount++;
            PayloadRecovery recovery = new PayloadRecovery(payload, highPriority, msgType);
            try
            {
                using (SocketAccessToken token = highPriority
                                               ? socketHelper.GetHighPrioritySendAccess()
                                               : socketHelper.GetSendAccess())
                {
                    token.OnCancellation(x => ResendPayloadAsync(x), recovery); // TODO : проверить на работоспособность
                    if (chunksCount == 1)
                    {
                        await socket.SendAsync(new ArraySegment<byte>(jsonBytes), msgType, true, token.CancellationToken);
                    }
                    else
                    {
                        for (int i = 0; i < chunksCount; i++)
                        {
                            bool isLastMsg = i == chunksCount;
                            int offset = i * chunkSize,
                                count = isLastMsg ? (jsonBytes.Length - chunkSize * i) : chunkSize;
                            await socket.SendAsync(new ArraySegment<byte>(jsonBytes, offset, count), msgType, isLastMsg, token.CancellationToken);
                        }
                    }
                }
            }
            catch(OperationCanceledException ex)
            { 
                // TODO : логирование
                return;
            }
            catch(WebSocketException ex)
            {
                Reconnect();
                return;
            }
            Interlocked.Increment(ref payloadSentLastMinute);
        }
        private async Task ResendPayloadAsync(object recoveryData)
        {
            PayloadRecovery recovery = recoveryData as PayloadRecovery;
            await SendAsync(recovery.Payload, recovery.IsHighPriority, recovery.MessageType, CancellationToken.None);
        }
        #endregion
        #region Constructor's
        internal Gateway(Uri gatewayUri, string botToken)
        {//TAI : размер стэков потоков
            this.botToken = botToken;
            this.gatewayUri = gatewayUri;
            socketHelper = new SocketLocker();
            socket = new ClientWebSocket();
            ReachedRateLimit += OnLimitReached;
            Zombied += Reconnect;
            heart = new Task(Heartbeat);
            heart.ConfigureAwait(false);
            socketListener = new Task(ListenToSocket);
            socketListener.ConfigureAwait(false);
            rateLimitListener = new Task(ListenToRateLimit);
            rateLimitListener.ConfigureAwait(false);
        }
        #endregion
        private class PayloadRecovery
        {
            internal GatewayPayload Payload { get; }
            internal bool IsHighPriority { get; }
            internal WebSocketMessageType MessageType { get; }
            internal PayloadRecovery(GatewayPayload payload, bool highPriority, WebSocketMessageType msgType)
            {
                Payload = payload;
                IsHighPriority = highPriority;
                MessageType = msgType;
            }
        }
    }
}
