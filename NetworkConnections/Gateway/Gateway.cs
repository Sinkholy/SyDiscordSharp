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
using static Gateway.DispatchEventHandler;

namespace Gateway
{
    internal class Gateway
    {
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
        private Task heart;
        private Uri gatewayUri;
        private ClientWebSocket socket;
        private readonly SocketLocker socketHelper;
        private TimeSpan heartbeatRate;
        private int payloadSentLastMinute = 0;
        private bool heartbeatAckReceived = true;
        private string sessionIdentifier,
                       botToken,
                       lastSequence;
        private int chunkSize = 1024 * 4, //TODO : подтягивать из конфига, 
                    messageCount = 0,
                    messageTotalLength = 0;
        private CancellationTokenSource heartCTS;
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
                    //Здесь может возникнуть вопрос "А почему мы используем ограничение равное 118?"
                    //Ответом на этот вопрос будет то, что ограничение отправляемых к Gateway дискорда 
                    //Равняется 120 запросов\минута. Но некоторые запросы, такие как Hearbeat 
                    //Необходимо отправлять каждые, в данный момент времени, 41.5 секунд.
                    //Для решения это проблемы я ввёл ограничение на 118 запросов, чтобы дабы
                    //2 запроса остаилсь в запасе для приоритетных запросов.
                    if (payloadSentLastMinute == limit - 2) //TODO : вынести хардлок в конфиг?
                    {
                        using (socketHelper.SendingSoftLock())
                        {
                            while (stopwatch.ElapsedMilliseconds < oneMinute)
                            {
                                if(payloadSentLastMinute == limit)
                                {
                                    using (socketHelper.SendingHardLock())
                                    {
                                        TimeSpan msToWait = TimeSpan.FromMilliseconds(oneMinute - stopwatch.ElapsedMilliseconds); //Через этот отрезок времени наступит новая минута => можно сбрасывать счётчик;
                                        ReachedRateLimit(msToWait);
                                        await Task.Delay(msToWait);
                                        break;
                                    }
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
            WebSocketReceiveResult result;
            while (true) //TAI : проверить разность быстродействия при обнулении буффера и без обнуления
            {
                //buffer = new byte[chunkSize]; //TAI : нужно ли здесь обнуление?
                jsonResultBuilder.Clear();
                jsonResultBuilder.Capacity = CalculateJsonBuilderCapacity(jsonResultBuilder.Length);
                using (socketHelper.GetReceiveAccess())
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
                                         .ConfigureAwait(false); // TAI : CTS
                    jsonResultBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    while (!result.EndOfMessage)
                    {
                        //buffer = new byte[chunkSize]; //TAI : нужно ли здесь обнуление?
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
                        jsonResultBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                }
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    SocketCloseReceived(result.CloseStatus, result.CloseStatusDescription);
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
                    await SendAsync(payload, true, WebSocketMessageType.Text, CancellationToken.None);
                    heartbeatAckReceived = false;
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
        internal void OnSequenceReceived(string sequence) => lastSequence = sequence;
        internal void OnReady(object sender, EventHandlerArgs args)
        {
            Ready ready = args.EventData as Ready;
            sessionIdentifier = ready.SessionIdentifier;
        }
        internal void OnReconnectRequested(IGatewayDataObject payload)
        {
            Reconnect();
        }
        #endregion
        #region Private method's
        private async void Reconnect()
        {
            heartCTS.Cancel();
            using (socketHelper.ReceivingLock())
            using (socketHelper.SendingHardLock())
            {
                socket.Abort();
                socket = new ClientWebSocket();
                await socket.ConnectAsync(gatewayUri, CancellationToken.None);
                await SendResume();
            }
            ImplantNewHeart();
        }
        private void ImplantNewHeart()
        {
            heartCTS = new CancellationTokenSource();
            heart = new Task(Heartbeat, heartCTS.Token);
            heart.ConfigureAwait(false);
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
                DiscordGatewayClient.RaiseLog("Cannot connect to gateway. Retrying in 5s");
                await ConnectToGatewayAsync();
                return;
            }
            rateLimitListener.Start();
            socketListener.Start();
        }
        internal async Task SendAsync(GatewayPayload payload, bool highPriority, WebSocketMessageType msgType, CancellationToken cts)
        {
            Console.WriteLine($"Sending: {payload.Opcode}");
            string jsonPayload = JsonConvert.SerializeObject(payload, typeof(GatewayPayload), null); // TODO : сериализацию с помощью внещнего проекта
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);

            short chunksCount = 0;
            for (int dataLength = jsonBytes.Length; dataLength > 0; dataLength -= chunkSize)
                chunksCount++;

            using (socketHelper.GetSendAccess(highPriority))
            {
                if (chunksCount == 1)
                {
                    await socket.SendAsync(new ArraySegment<byte>(jsonBytes), msgType, true, cts);
                }
                else
                {
                    for (int i = 0; i < chunksCount; i++)
                    {
                        bool isLastMsg = i == chunksCount;
                        int offset = i * chunkSize,
                            count = isLastMsg ? (jsonBytes.Length - chunkSize * i) : chunkSize;
                        await socket.SendAsync(new ArraySegment<byte>(jsonBytes, offset, count), msgType, isLastMsg, cts);
                    }
                }
            }
            Interlocked.Increment(ref payloadSentLastMinute);
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
            ImplantNewHeart();
            socketListener = new Task(ListenToSocket);
            socketListener.ConfigureAwait(false);
            rateLimitListener = new Task(ListenToRateLimit);
            rateLimitListener.ConfigureAwait(false);
        }
        #endregion
    }
}
