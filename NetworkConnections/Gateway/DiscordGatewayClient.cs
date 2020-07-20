using Gateway.DataObjects;
using Gateway.Entities.Guilds;
using Gateway.Entities.Users;
using Gateway.Payload.DataObjects;
using Gateway.Payload.DataObjects.Dispatch;
using Gateway.Payload.DataObjects.Dispatch.DispatchEvents;
using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Gateway.DispatchEventHandler;

namespace Gateway
{
    public class DiscordGatewayClient //TODO : sharding
    {
        #region Static fields\properties
        public static DiscordGatewayClient GetInstance() //TAI : потокобезопасность
        {
            if (instance == null)
            {
                instance = new DiscordGatewayClient();
                return instance;
            }
            else
            {
                return instance;
            }
        }
        internal static void RaiseLog(string logData)
        {
            GetInstance().Log(logData);
        }
        internal static IGuild TryToGetGuild(string id)
        {
            DiscordGatewayClient client = GetInstance();
            if (client.guilds.ContainsKey(id))
                return client.guilds[id];
            else
                return null;
        }
        private static DiscordGatewayClient instance;
        #endregion
        #region Public fields
        public TimeSpan Uptime => DateTime.Now - readyReceived;
        public IReadOnlyCollection<IGuild> Guilds
            => guilds.Values.ToList() as IReadOnlyCollection<IGuild>;
        #endregion
        #region Public events
        public delegate void ToLog(string logData);
        public event ToLog Log = delegate { };
        #endregion
        #region Internal fields
        internal Dictionary<string, IGuild> guilds { get; private set; }
        internal Dictionary<string, List<string>> userGuilds { get; private set; }
        internal User BotUser { get; private set; }
        internal Uri GatewayUri { get; private set; }//RO?
        #endregion
        #region Internal events
        internal delegate void VoidEvent();
        internal delegate void NewSequence(string seq);
        internal delegate void NewClientEvent(string eventName, string eventData);
        internal delegate void NewSystemEvent(Opcode opcode, IGatewayDataObject data);

        internal event NewClientEvent NewClientEventReceived = delegate { };
        internal event NewSystemEvent NewSystemEventReceived = delegate { };
        internal event NewSequence NewSequenceReceived = delegate { };
        #endregion
        #region Private fields
        private readonly string botToken = "NTU5MDkwMTUzOTM1NjAxNjY1.XtKPog.7epgH4xS8QxLqGgiGyBLCladnyI"; //TODO : перенести токен в конфиг и объеденить методы с API
        private readonly JsonSerializer jsonSerializer;
        private Gateway gateway;
        private readonly SystemEventHandler systemEventHandler;
        private readonly DispatchEventHandler dispatchEventHandler;
        private short identifyLimit; // TODO : метод обновлящий значение при отправке новой идентификации
                                     // и изначальное записывание значения полученое при первичном запросе к HTTP API
        private DateTime readyReceived;
        #endregion
        #region Event handlers
        private void OnNewPayloadReceivedAsync(string payloadStr)
        {
            GatewayPayload payload = JsonConvert.DeserializeObject<GatewayPayload>(payloadStr);
            if (payload.Opcode == Opcode.Dispatch)
            {
                NewClientEventReceived(payload.EventName, (payload.Data as Dispatch).EventData);
            }
            else
            {
                NewSystemEventReceived(payload.Opcode, payload.Data);
            }
        }
        private void OnConnection(IGatewayDataObject payload) => Console.WriteLine(1);//TODO : do smth
        private void OnReady(object sender, EventHandlerArgs args)
        {
            Ready ready = args.EventData as Ready;
            BotUser = ready.User;
            foreach(var guild in ready.Guilds) 
                guilds.Add(guild.Identifier, guild as IGuild);
            readyReceived = DateTime.Now;
        }
        private void OnGuildCreated(object sender, EventHandlerArgs args)
        {
            IGuild guild = args.EventData as IGuild;
            if (guilds.ContainsKey(guild.Identifier))
                guilds[guild.Identifier].UpdateGuild(guild);
            else
                guilds.Add(guild.Identifier, guild);
            
        }
        #endregion
        #region Public methods
        public async Task StartAsync(Uri gatewayUri) //TAI : подписать этот метод на некое событие в HTTP-клиенте сигнализирующее о получении /gateway ответа 
        {
            GatewayUri = gatewayUri;
            gateway = new Gateway(GatewayUri, botToken);

            #region Event's handler's binding
            NewSequenceReceived += gateway.OnSequenceReceived;
            NewClientEventReceived += dispatchEventHandler.OnNewClientEventReceived;
            NewSystemEventReceived += systemEventHandler.OnNewSystemEventReceived;

            systemEventHandler.Connected += gateway.OnConnection;
            systemEventHandler.Connected += OnConnection;
            systemEventHandler.HeartbeatACK += gateway.OnHeartbeatAck;

            dispatchEventHandler.GuildCreated += OnGuildCreated;
            dispatchEventHandler.Ready += OnReady;
            dispatchEventHandler.Ready += gateway.OnReady;

            gateway.NewPayloadReceived += OnNewPayloadReceivedAsync;
            #endregion
            await gateway.ConnectToGatewayAsync();
            await AuthorizeAsync();
        }
        public IGuild[] GetUserGuilds(IUser user) => GetUserGuilds(user.Identifier);
        public IGuild[] GetUserGuilds(string userId)
        {
            List<string> guildsIdentifiers = userGuilds[userId];
            List<IGuild> guilds = new List<IGuild>();
            foreach (var guild in guildsIdentifiers)
            {
                guilds.Add(this.guilds[guild]);
            }
            return guilds.ToArray();
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
            Identify identityObj = new Identify(botToken, properties, IdentifyIntents.None);
            GatewayPayload payload = new GatewayPayload(Opcode.Identify, identityObj);
            await gateway.SendAsync(payload, WebSocketMessageType.Text, CancellationToken.None);
        }
        #endregion
        #region Ctor's
        private DiscordGatewayClient() 
        {
            guilds = new Dictionary<string, IGuild>();
            jsonSerializer = new JsonSerializer();
            systemEventHandler = new SystemEventHandler();
            dispatchEventHandler = new DispatchEventHandler();
        }
        #endregion
    }
}
