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
        private readonly Task socketListener,
                                rateLimitListener,
                                heart;
        private Uri gatewayUri;
        private readonly ClientWebSocket socket;
        private readonly SemaphoreSlim socketSemaphore = new SemaphoreSlim(1);
        private TimeSpan heartbeatRate;
        private byte payloadSentLastMinute = 0;
        private bool heartbeatAckReceived = true;
        private string sessionIdentifier,
                       botToken,
                       lastSequence;
        private int chunkSize = 1024 * 4, //TODO : подтягивать из конфига, 
                    messageCount = 0,
                    messageTotalLength = 0;
        #region Delegates and events
        private delegate void socketCloseReceived(WebSocketCloseStatus? status, string description);
        private event socketCloseReceived SocketCloseReceived = delegate { };
        
        internal delegate void lastSequenceChanged(string oldSequence, string newSequence);
        internal delegate void payloadReceived(string payloadJson);
        internal delegate void reachedRateLimit(TimeSpan tillNextMinute);
        internal delegate void voidEvent();
        internal event payloadReceived NewPayloadReceived = delegate { };
        internal event voidEvent Zombied = delegate { };
        internal event reachedRateLimit ReachedRateLimit = delegate { };
        #endregion

        #region Thread's tasks
        private async void ListenToRateLimit()
        {
            int oneMinute = 60000; //From ms 
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                while (stopwatch.ElapsedMilliseconds < oneMinute)
                {
                    if (payloadSentLastMinute > 118)
                    {
                        socketSemaphore.Wait();
                        TimeSpan msToWait = TimeSpan.FromMilliseconds(oneMinute - stopwatch.ElapsedMilliseconds); //Через этот отрезок времени наступит новая минута => можно сбрасывать счётчик;
                        ReachedRateLimit(msToWait);
                        await Task.Delay(msToWait);
                        socketSemaphore.Release();
                        break;
                    }
                }
                payloadSentLastMinute = 0;
                stopwatch.Restart();
            }
        }
        private async void ListenToSocket()
        {
            byte[] buffer = new byte[chunkSize];
            int capacity = 256; // TODO : изначальный размер буффера
            StringBuilder jsonResultBuilder = new StringBuilder(capacity);
            while (true) //TAI : проверить разность быстродействия при обнулении буффера и без обнуления
            {
                //buffer = new byte[chunkSize]; //TAI : нужно ли здесь обнуление?
                jsonResultBuilder.Clear();
                jsonResultBuilder.Capacity = CalculateJsonBuilderCapacity(jsonResultBuilder.Length);
                WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false); // TAI : CTS
                socketSemaphore.Wait();
                Console.WriteLine("Gateway thread ID: " + Thread.CurrentThread.ManagedThreadId);
                jsonResultBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                while (!result.EndOfMessage)
                {
                    //buffer = new byte[chunkSize]; //TAI : нужно ли здесь обнуление?
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
                    jsonResultBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    SocketCloseReceived(result.CloseStatus, result.CloseStatusDescription);
                }
                else
                {
                    socketSemaphore.Release();
#pragma warning disable 4014
                    string payload = jsonResultBuilder.ToString();
                    Task.Factory.StartNew(() => NewPayloadReceived(payload))
                                .ConfigureAwait(false);
#pragma warning disable 4014
                }
            }
        }
        private async void Heartbeat()
        {
            Heartbeat heartbeatObj = new Heartbeat(lastSequence);
            GatewayPayload payload = new GatewayPayload(Opcode.Heartbeat, heartbeatObj);
            while (true)
            {
                if (heartbeatAckReceived)
                {
                    heartbeatObj.Sequence = lastSequence;
                    await SendAsync(socket, payload, WebSocketMessageType.Text, CancellationToken.None);
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
        #endregion
        #region Private method's
        private async void Reconnect() //TODO : доделать метод восстановления
        {//TODO : нужен механизм останаливающий\запускающий сердце
            //heart.Suspend();
            socketSemaphore.Wait();
            socket.Abort();
            await socket.ConnectAsync(gatewayUri, CancellationToken.None);
            Resume resumeObj = new Resume(botToken, sessionIdentifier, lastSequence);
            GatewayPayload payload = new GatewayPayload(Opcode.Resume, resumeObj);
            await SendAsync(socket, payload, WebSocketMessageType.Text, CancellationToken.None);
            socketSemaphore.Release();
            //heart.Resume();
        }
        private int CalculateJsonBuilderCapacity(int builderLenth) //TODO : Считать медиану
        {
            messageCount++;
            messageTotalLength += builderLenth;
            return 200;
        }
        #endregion
        #region Internal method's
        internal async Task ConnectToGatewayAsync()
        {
            await socket.ConnectAsync(gatewayUri, CancellationToken.None);
            if (socket.State != WebSocketState.Open)
            {
                throw new Exception("cannot connect"); // TODO : попытку переподключения
            }
            rateLimitListener.Start();
            socketListener.Start();
        }
        internal async Task SendAsync(ClientWebSocket client, GatewayPayload payload, WebSocketMessageType msgType, CancellationToken cts)
        {
            string jsonPayload = JsonConvert.SerializeObject(payload, typeof(GatewayPayload), null); // TODO : сериализацию с помощью внещнего проекта
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonPayload);

            short chunksCount = 0;
            for (int dataLength = jsonBytes.Length; dataLength > 0; dataLength -= chunkSize)
                chunksCount++;

            if (chunksCount == 1)
            {
                socketSemaphore.Wait();
                await client.SendAsync(new ArraySegment<byte>(jsonBytes), msgType, true, cts);
            }
            else
            {
                for (int i = 0; i < chunksCount; i++)
                {
                    bool isLastMsg = i == chunksCount ? true : false;
                    int offset = i * chunkSize,
                        count = isLastMsg ? (jsonBytes.Length - chunkSize * i) : chunkSize;
                    socketSemaphore.Wait();
                    await client.SendAsync(new ArraySegment<byte>(jsonBytes, offset, count), msgType, isLastMsg, cts);
                }
            }
            payloadSentLastMinute++;
            socketSemaphore.Release();
        }
        #endregion
        #region Constructor's
        internal Gateway(ClientWebSocket socket, Uri gatewayUri, string botToken)
        {//TAI : размер стэков потоков
            this.botToken = botToken;
            this.gatewayUri = gatewayUri;
            this.socket = socket;
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
    }
}
