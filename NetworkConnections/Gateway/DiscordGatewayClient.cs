using Gateway.DataObjects;
using Gateway.Entities.Guilds;
using Gateway.Entities.Users;
using Gateway.Payload.DataObjects;
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
        #endregion
        #region Internal fields
        internal IReadOnlyCollection<IGuild> Guilds
            => guilds.Values.ToList() as IReadOnlyCollection<IGuild>;
        internal User BotUser;
        internal Uri GatewayUri { get; private set; }//RO?
        internal Dictionary<string, IGuild> guilds;
        #endregion
        #region Private fields
        private readonly ConcurrentQueue<GatewayPayload> payloadsReceived;
        private readonly string botToken = "NTU5MDkwMTUzOTM1NjAxNjY1.XtKPog.7epgH4xS8QxLqGgiGyBLCladnyI"; //TODO : перенести токен в конфиг и объеденить методы с API
        private readonly JsonSerializer jsonSerializer;
        private Gateway gateway;
        private readonly SystemEventHandler payloadHandler;
        private readonly DispatchEventHandler eventHandler;
        private ClientWebSocket clientWebSocket;
        private short identifyLimit; // TODO : метод обновлящий значение при отправке новой идентификации
                                     // и изначальное записывание значения полученое при первичном запросе к HTTP API
        private DateTime readyReceived;
        private Dictionary<string, List<string>> userGuilds;
        #endregion
        #region Event handlers
        private void OnConnection() => Console.WriteLine(1);//TODO : do smth
        private void OnReady(object sender, EventHandlerArgs args)
        {
            Ready ready = args.EventData as Ready;
            BotUser = ready.User;
            foreach(var guild in ready.Guilds) 
                guilds.Add(guild.Identifier, guild as IGuild);
            readyReceived = DateTime.Now;
        }
        private void OnGuildCreate(object sender, EventHandlerArgs args)
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
            gateway = new Gateway(clientWebSocket, GatewayUri, botToken);

            #region Event's handler's binding
            payloadHandler.Connected += gateway.OnConnection;
            payloadHandler.HeartbeatACK += gateway.OnHeartbeatAck;
            payloadHandler.NewSequenceReceived += gateway.OnSequenceReceived;
            payloadHandler.NewEvent += eventHandler.OnNewEventCreated;
            eventHandler.GuildCreated += OnGuildCreate;
            eventHandler.Ready += OnReady;
            eventHandler.Ready += gateway.OnReady;
            gateway.NewPayloadReceived += AddToQueue;
            #endregion
            await gateway.ConnectToGatewayAsync();
            await AuthorizeAsync();
        }
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
        public IGuild[] GetUserGuilds(IUser user) => GetUserGuilds(user.Identifier);
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
            await gateway.SendAsync(clientWebSocket, payload, WebSocketMessageType.Text, CancellationToken.None);
        }
        private void AddToQueue(string eventData)
        {
            GatewayPayload payload = DeserializeJson<GatewayPayload>(eventData);
            payloadsReceived.Enqueue(payload);
        }
        #endregion
        #region Ctor's
        private DiscordGatewayClient() 
        {
            guilds = new Dictionary<string, IGuild>();
            payloadsReceived = new ConcurrentQueue<GatewayPayload>();
            clientWebSocket = new ClientWebSocket();
            jsonSerializer = new JsonSerializer();
            payloadHandler = new SystemEventHandler(payloadsReceived);
            eventHandler = new DispatchEventHandler();
        }
        #endregion
    }
}
