using Gateway.DataObjects;
using Gateway.DataObjects.Guilds;
using Gateway.DataObjects.Users;
using Gateway.Enums;
using Gateway.Payload.DataObjects;
using Gateway.Payload.EventObjects;
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

namespace Gateway
{
    public class DiscordGatewayClient //TODO : sharding
    {
        private readonly ConcurrentQueue<GatewayPayload> payloadsReceived;
        private readonly string botToken = "NTU5MDkwMTUzOTM1NjAxNjY1.XtKPog.7epgH4xS8QxLqGgiGyBLCladnyI"; //TODO : перенести токен в конфиг и объеденить методы с API
        private readonly JsonSerializer jsonSerializer;
        private Gateway gateway;
        private readonly PayloadHandler payloadHandler;
        private readonly EventHandler eventHandler;
        private ClientWebSocket clientWebSocket;
        private short identifyLimit; // TODO : метод обновлящий значение при отправке новой идентификации
                                    // и изначальное записывание значения полученое при первичном запросе к HTTP API
        private DateTime readyReceived;
        private Dictionary<string, IGuild> guilds;
        private Dictionary<string, List<string>> userGuilds;

        internal IGuild[] Guilds => guilds.Values.ToArray();
        internal User BotUser;
        internal Uri GatewayUri { get; private set; }//RO?
        public TimeSpan Uptime => DateTime.Now - readyReceived;

        #region Event handlers
        private void OnConnection() => Console.WriteLine(1);//TODO : do smth
        private void OnReady(IEvent eventData)
        {
            Ready ready = eventData as Ready;
            BotUser = ready.User;
            foreach(var guild in ready.Guilds) 
                guilds.Add(guild.Identifier, guild as IGuild);
            readyReceived = DateTime.Now;
        }
        private void OnGuildCreate(IEvent eventData)
        {
            IGuild guild = eventData as IGuild;
            if (guilds.ContainsKey(guild.Identifier)) 
                guilds.Add(guild.Identifier, guild);
            else
                guilds[guild.Identifier].UpdateGuild(guild);
        }
        #endregion
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
        public async Task StartAsync(Uri gatewayUri) //TAI : подписать этот метод на некое событие в HHTP-клиенте сигнализирующее о получении /gateway ответа 
        {
            GatewayUri = gatewayUri;
            gateway = new Gateway(clientWebSocket, GatewayUri, botToken);
            #region Event's handler's binding
            payloadHandler.Connected += gateway.OnConnection;
            payloadHandler.HeartbeatACK += gateway.OnHeartbeatAck;
            payloadHandler.NewSequenceReceived += gateway.OnSequenceReceived;
            payloadHandler.NewEvent += eventHandler.OnNewEventCreated;
            eventHandler.GuildCreate += OnGuildCreate;
            #region ready
            eventHandler.Ready += OnReady;
            eventHandler.Ready += gateway.OnReady;
            #endregion

            gateway.NewPayload += AddToQueue;
            #endregion
            await gateway.ConnectToGatewayAsync();
            await AuthorizeAsync();
        }
        public IGuild[] GetUserGuilds(string userId)
        {
            List<string> guildsIdentifiers = userGuilds[userId];
            List<IGuild> guilds = new List<IGuild>();
            foreach(var guild in guildsIdentifiers)
            {
                guilds.Add(this.guilds[guild]);
            }
            return guilds.ToArray();
        }
        public IGuild[] GetUserGuilds(IUser user) => GetUserGuilds(user.Identifier);
        public DiscordGatewayClient() 
        {
            guilds = new Dictionary<string, IGuild>();
            payloadsReceived = new ConcurrentQueue<GatewayPayload>();
            clientWebSocket = new ClientWebSocket();
            jsonSerializer = new JsonSerializer();
            payloadHandler = new PayloadHandler(payloadsReceived);
            eventHandler = new EventHandler();
        }
    }
}

