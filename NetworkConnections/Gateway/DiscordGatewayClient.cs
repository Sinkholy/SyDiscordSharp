using Gateway.DataObjects;
using Gateway.Entities;
using Gateway.Entities.Channels;
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
using System.Runtime.Serialization;
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
        public delegate void StringEvent(string guildId);

        public event StringEvent BotRemovedFromGuild = delegate { };
        public event StringEvent GuildBecameUnavailable = delegate { };
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
                guilds[guild.Identifier] = guild;
            else
                guilds.Add(guild.Identifier, guild);

        }
        private void OnGuildUpdated(object sender, EventHandlerArgs args)
        {
            Guild guild = args.EventData as Guild;
            (guilds[guild.Identifier] as Guild).UpdateGuild(guild);
        }
        private void OnGuildDeleted(object sender, EventHandlerArgs args)
        {
            GuildPreview guild = args.EventData as GuildPreview;
            if (guild.Unavailable)
            {
                (guilds[guild.Identifier] as Guild).Unavailable = true;
                GuildBecameUnavailable(guild.Identifier); //TODO : проверка была ли удалена гильдия при помощи отправки GET запроса
            }
            else
            {
                BotRemovedFromGuild(guild.Identifier);
            }
        }
        private void OnChannelCreated(object sender, EventHandlerArgs args)
        {
            IGuildChannel newChannel = args.EventData as IGuildChannel;
            if (!guilds.ContainsKey(newChannel.GuildIdentifier))
                Log("New channel was created but no guild stored for this channel");
            else
            {
                if(guilds[newChannel.GuildIdentifier] is Guild guild)
                {
                    guild.AddChannel(newChannel);
                }
                else
                {
                    Log("Cannot cast target IGuild to Guild");
                }
            }
        }
        private void OnChannelUpdated(object sender, EventHandlerArgs args)
        {
            IGuildChannel newChannelInfo = args.EventData as IGuildChannel;
            if (!guilds.ContainsKey(newChannelInfo.GuildIdentifier))
                Log("Handling ChannelUpdate event. Cannot find target guild");
            else
            {
                if (guilds[newChannelInfo.GuildIdentifier] is Guild guild)
                {
                    IChannel targetChannel = guild.TryToGetChannel(newChannelInfo.Identifier);
                    if(targetChannel is null)
                    {
                        Log("Handling ChannelUpdate event. Cannot find target channel");
                        return;
                    }
                    Console.WriteLine((targetChannel as IUpdatableChannel).UpdateChannel(newChannelInfo));
                }
                else
                {
                    Log("Handling ChannelUpdate event. Cannot cast target IGuild to Guild");
                }
            }
        }
        private void OnChannelDeleted(object sender, EventHandlerArgs args)
        {
            IGuildChannel channelToDelete = args.EventData as IGuildChannel;
            if (!guilds.ContainsKey(channelToDelete.GuildIdentifier))
                Log("Handling ChannelDeleted event. Cannot find target guild");
            else
            {
                if (guilds[channelToDelete.GuildIdentifier] is Guild guild)
                {
                    guild.RemoveChannel(channelToDelete.Identifier);
                }
                else
                {
                    Log("Handling ChannelUpdate event. Cannot cast target IGuild to Guild");
                }
            }
        }
        private void OnRoleCreated(object sender, EventHandlerArgs args)
        {
            RoleEvent createdRole = args.EventData as RoleEvent;
            if (!guilds.ContainsKey(createdRole.GuildIdentifier))
                Log("Handling RoleCreated event. Cannot find target guild");
            else
            {
                if (guilds[createdRole.GuildIdentifier] is Guild guild)
                {
                    guild.AddRole(createdRole.Role);
                }
                else
                {
                    Log("Handling RoleCreated event. Cannot cast target IGuild to Guild");
                }
            }
        }
        private void OnRoleUpdated(object sender, EventHandlerArgs args)
        {
            RoleEvent updatedRole = args.EventData as RoleEvent;
            if (!guilds.ContainsKey(updatedRole.GuildIdentifier))
                Log("Handling RoleUpdated event. Cannot find target guild");
            else
            {
                if (guilds[updatedRole.GuildIdentifier] is Guild guild)
                {
                    Role roleToUpdate = guild.TryToGetRole(updatedRole.Role.Identifier);
                    if(roleToUpdate is null)
                    {
                        Log("Handling RoleUpdated event. Cannot find target role");
                    }
                    else
                    {
                        roleToUpdate.UpdateRole(updatedRole.Role);
                    }
                }
                else
                {
                    Log("Handling RoleUpdated event. Cannot cast target IGuild to Guild");
                }
            }
        }
        private void OnRoleDeleted(object sender, EventHandlerArgs args)
        {
            RoleDeletedEvent deletedRole = args.EventData as RoleDeletedEvent;
            if (!guilds.ContainsKey(deletedRole.GuildIdentifier))
                Log("Handling RoleDeleted event. Cannot find target guild");
            else
            {
                if (guilds[deletedRole.GuildIdentifier] is Guild guild)
                {
                    guild.RemoveRole(deletedRole.RoleIdentifier);
                }
                else
                {
                    Log("Handling RoleDeleted event. Cannot cast target IGuild to Guild");
                }
            }
        }
        private void OnGuildUserAdded(object sender, EventHandlerArgs args)
        {
            GuildUser newUser = args.EventData as GuildUser;
            if (guilds.ContainsKey(newUser.GuildIdentifier))
            {
                if (guilds[newUser.GuildIdentifier] is Guild guild)
                {
                    guild.AddUser(newUser);
                }
                else
                {
                    Log("Handling GuildUserAdded event. Cannot cast target IGuild to Guild");
                }
            }
            else
            {
                Log("Handling GuildUserAdded event. Cannot cast target IGuild to Guild");
            }
        }
        private void OnGuildUserUpdated(object sender, EventHandlerArgs args)
        {
            GuildUser newUserInfo = args.EventData as GuildUser;
            if (guilds.ContainsKey(newUserInfo.GuildIdentifier))
            {
                if (guilds[newUserInfo.GuildIdentifier] is Guild guild)
                {
                    Console.WriteLine((guild.TryToGetUser(newUserInfo.Identifier) as GuildUser).Update(newUserInfo));
                }
                else
                {
                    Log("Handling GuildUserUpdated event. Cannot cast target IGuild to Guild");
                }
            }
            else
            {
                Log("Handling RoleDeleted event. Cannot find target guild");
            }
        }
        private void OnGuildUserRemoved(object sender, EventHandlerArgs args)
        {
            GuildMember deletedUser = args.EventData as GuildMember;
            if (guilds.ContainsKey(deletedUser.GuildIdentifier))
            {
                if (guilds[deletedUser.GuildIdentifier] is Guild guild)
                {
                    guild.RemoveUser(deletedUser.User.Identifier);
                }
                else
                {
                    Log("Handling GuildUserUpdated event. Cannot cast target IGuild to Guild");
                }
            }
            else
            {
                Log("Handling UserDeleted event. Cannot cast target IGuild to Guild");
            }
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
            dispatchEventHandler.GuildUpdated += OnGuildUpdated;
            dispatchEventHandler.GuildDeleted += OnGuildDeleted;
            dispatchEventHandler.ChannelCreated += OnChannelCreated;
            dispatchEventHandler.ChannelUpdated += OnChannelUpdated;
            dispatchEventHandler.ChannelDeleted += OnChannelDeleted;
            dispatchEventHandler.GuildRoleCreated += OnRoleCreated;
            dispatchEventHandler.GuildRoleUpdated += OnRoleUpdated;
            dispatchEventHandler.GuildRoleDeleted += OnRoleDeleted;
            dispatchEventHandler.GuildMemberAdded += OnGuildUserAdded;
            dispatchEventHandler.GuildMemberUpdated += OnGuildUserUpdated;
            dispatchEventHandler.GuildMemberRemoved += OnGuildUserRemoved;
            dispatchEventHandler.Ready += OnReady;
            dispatchEventHandler.Ready += gateway.OnReady;

            Log += (x) => Console.WriteLine(x);

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
