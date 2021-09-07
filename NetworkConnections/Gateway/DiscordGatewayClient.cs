using DiscordDataObjects.Users.Activities;

using Gateway.Payload.DataObjects;
using Gateway.Payload.DataObjects.Dispatch;
using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SyDiscordSharp")]
namespace Gateway
{
    internal class DiscordGatewayClient //TODO : sharding
    {
        #region Internal fields\props
        internal delegate void ToLog(string logData);
        internal event ToLog Log = delegate { };
        internal SystemEventHandler SystemEventHandler { get; }
        internal DispatchEventHandler DispatchEventHandler { get; }
        #endregion
        #region Internal events
        internal delegate void VoidEvent();
        internal delegate void NewSequence(int seq);
        internal delegate void NewDispatchEvent(string eventName, string eventData);
        internal delegate void NewSystemEvent(Opcode opcode, IGatewayDataObject data);

        internal event NewDispatchEvent NewClientEventReceived = delegate { };
        internal event NewSystemEvent NewSystemEventReceived = delegate { };
        internal event NewSequence NewSequenceReceived = delegate { };
        #endregion
        internal int SessionsCount => gateway.SessionNumber;
        internal TimeSpan SessionUptime => DateTime.Now - sessionStarted;
        private DateTime sessionStarted;

        #region Private fields\props
        private string botToken;
        private readonly JsonSerializer jsonSerializer;
        private Gateway gateway;
        private Uri GatewayUri;
        private short identifyLimit; // TODO : метод обновлящий значение при отправке новой идентификации
                                     // и изначальное записывание значения полученое при первичном запросе к HTTP API
        #endregion
        #region Event handlers
        private void OnReady(object sender, EventHandlerArgs args)
        {
            sessionStarted = DateTime.Now;
        }
        private void OnNewPayloadReceivedAsync(string payloadStr)
        {
            GatewayPayload payload = JsonConvert.DeserializeObject<GatewayPayload>(payloadStr);
            if (payload.Sequence != null)
            {
                NewSequenceReceived(payload.Sequence.Value);
            }
            if (payload.Opcode == Opcode.Dispatch)
            {
                NewClientEventReceived(payload.EventName, (payload.Data as Dispatch).EventData);
            }
            else
            {
                NewSystemEventReceived(payload.Opcode, payload.Data);
            }
        }
        private async void OnInvalidSessionRecieved(IGatewayDataObject data)
        {
            // В документации дискорда сказано, что при получении InvaidSession необходимо
            // подождать 2-5 секунд и отправить Identify.
            await Task.Delay(5000);
            await AuthorizeAsync();
        }
        #endregion
        #region Internal methods
        internal async Task StartAsync(Uri gatewayUri, string botToken) //TAI : подписать этот метод на некое событие в HTTP-клиенте сигнализирующее о получении /gateway ответа 
        {
            this.botToken = botToken;
            GatewayUri = gatewayUri;
            gateway = new Gateway(GatewayUri, botToken);

            NewSequenceReceived += gateway.OnSequenceReceived;
            NewClientEventReceived += DispatchEventHandler.OnNewClientEventReceived;
            NewSystemEventReceived += SystemEventHandler.OnNewSystemEventReceived;
            SystemEventHandler.Connected += gateway.OnConnection;
            SystemEventHandler.HeartbeatACK += gateway.OnHeartbeatAck;
            SystemEventHandler.ReconnectRequested += gateway.OnReconnectRequested;
            SystemEventHandler.InvalidSession += OnInvalidSessionRecieved;
            SystemEventHandler.InvalidSession += gateway.OnInvalidSessionReceived;
            DispatchEventHandler.Ready += gateway.OnReady;
            DispatchEventHandler.Ready += OnReady;
            gateway.NewPayloadReceived += OnNewPayloadReceivedAsync;
            Log += (x) => Console.WriteLine(x);

            await gateway.ConnectToGatewayAsync();
            await AuthorizeAsync();
        }

        #endregion
        #region Private methods
        private string SerializeJson(object toSerialize)
        {
            var sb = new StringBuilder(256); //TODO: подумать над размером буффера 
            using (TextWriter tw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                jsonSerializer.Serialize(tw, toSerialize);
                return sb.ToString();

                //TODO : подумать над тем стоит ли использовать JsonWriter
                //Его преимущество заключается в том, что он как-то умно скипает парсинг (?) JToken 
                //в обще он имеет преимущества, как я понял, в скорости при работе с JToken и при работе с большими JSon-объектами

                //using (JsonWriter jw = new JsonTextWriter(text))
                //{
                //    jsonSerializer.Serialize(tw, toSerialize);
                //    return sb.toString();
                //}
            }
        }
        private TObject DeserializeJson<TObject>(string value)
        {
            return JsonConvert.DeserializeObject<TObject>(value); // TAI: он тут вообще нужен?
        }
        private async Task AuthorizeAsync()
        {
            IdentifyProperties properties = new IdentifyProperties("SinkholesImpl", "SinkholesDevice");
            IActivity presencesActivity = Activity.CreateGamingActivity("SomeShit");
            IdentifyPresence presences = new IdentifyPresence(UserStatus.Online, false, null, presencesActivity);
            Identify identityObj = new Identify(botToken, properties, IdentifyIntents.None, presences);
            GatewayPayload payload = new GatewayPayload(Opcode.Identify, identityObj);
            await gateway.SendAsync(payload, false, WebSocketMessageType.Text, CancellationToken.None);
        }
        #endregion
        #region Ctor's
        internal DiscordGatewayClient()
        {
            jsonSerializer = new JsonSerializer();
            SystemEventHandler = new SystemEventHandler();
            DispatchEventHandler = new DispatchEventHandler();
        }
        #endregion
    }
}