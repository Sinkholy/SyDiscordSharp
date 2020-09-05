using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using API;
using Gateway;
//using Discord;
//using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Diagnostics;
using Gateway.Entities.Guilds;
using Gateway.Entities.Invite;
using Gateway.Entities;
using Gateway.Entities.Users;
using Gateway.Payload.DataObjects.Dispatch.DispatchEvents;
using Gateway.Payload.DataObjects;
using Gateway.Entities.Message;
using Gateway.Entities.Channels.Text;
using Gateway.Entities.Channels;

namespace SyDiscordSharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DiscordClient client = new DiscordClient();
            await client.Start();
            Console.Read();
        }
        public class DiscordClient
        {
            #region Discord event's
            public event EventHandler<EventHandlerArgs> Ready = delegate { };
            public event EventHandler<EventHandlerArgs> GuildCreated = delegate { };
            public event EventHandler<EventHandlerArgs> GuildUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> GuildDeleted = delegate { };
            public event EventHandler<EventHandlerArgs> GuildRoleCreated = delegate { };
            public event EventHandler<EventHandlerArgs> GuildRoleUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> GuildRoleDeleted = delegate { };
            public event EventHandler<EventHandlerArgs> ChannelCreated = delegate { };
            public event EventHandler<EventHandlerArgs> ChannelUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> ChannelDeleted = delegate { };
            public event EventHandler<EventHandlerArgs> ChannelPinsUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> GuildMemberAdded = delegate { };
            public event EventHandler<EventHandlerArgs> GuildMemberUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> GuildMemberRemoved = delegate { };
            public event EventHandler<EventHandlerArgs> GuildMemberChunksReceived = delegate { };
            public event EventHandler<EventHandlerArgs> GuildBanAdded = delegate { };
            public event EventHandler<EventHandlerArgs> GuildBanRemoved = delegate { };
            public event EventHandler<EventHandlerArgs> GuildEmojisUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> GuildIntegrationsUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> InviteCreated = delegate { };
            public event EventHandler<EventHandlerArgs> InviteDeleted = delegate { };
            public event EventHandler<EventHandlerArgs> VoiceStateUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> PresenceUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> MessageCreated = delegate { };
            public event EventHandler<EventHandlerArgs> MessageUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> MessageDeleted = delegate { };
            public event EventHandler<EventHandlerArgs> MessageDeletedBulk = delegate { };
            public event EventHandler<EventHandlerArgs> MessageReactionAdded = delegate { };
            public event EventHandler<EventHandlerArgs> MessageReactionRemoved = delegate { };
            public event EventHandler<EventHandlerArgs> MessageReactionRemovedAll = delegate { };
            public event EventHandler<EventHandlerArgs> MessageReactionEmoji = delegate { };
            public event EventHandler<EventHandlerArgs> TypingStarted = delegate { };
            public event EventHandler<EventHandlerArgs> UserUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> VoiceServerUpdated = delegate { };
            public event EventHandler<EventHandlerArgs> WebhooksUpdated = delegate { };

            public delegate void StringEvent(string guildId);
            public event StringEvent BotRemovedFromGuild = delegate { };
            public event StringEvent GuildBecameUnavailable = delegate { };
            #endregion
            public delegate void ToLog(string logData);
            public event ToLog Log = delegate { };
            internal static void RaiseLog(string logData)
            {
                GetInstance().Log(logData);
            }
            #region Field's and prop's
            public IReadOnlyCollection<IGuild> Guilds
                => guilds.Values.ToList() as IReadOnlyCollection<IGuild>;
            public IUser BotUser { get; private set; }
            public TimeSpan Uptime => DateTime.Now - readyReceived;
            private Dictionary<string, IGuild> guilds;
            private Dictionary<string, List<string>> userGuilds;
            private readonly DiscordHttpClient httpClient;
            private readonly DiscordGatewayClient gatewayClient;
            private DateTime readyReceived;
            #endregion
            #region Singleton
            public static DiscordClient GetInstance() //TAI : потокобезопасность
            {
                if (instance == null)
                {
                    instance = new DiscordClient();
                    return instance;
                }
                else
                {
                    return instance;
                }
            }
            private static DiscordClient instance;
            #endregion

            internal IGuild TryToGetGuild(string id)
            {
                DiscordClient client = GetInstance();
                if (client.guilds.ContainsKey(id))
                    return client.guilds[id];
                else
                    return null;
            }
            internal IGuild[] GetUserGuilds(IUser user) => GetUserGuilds(user.Identifier);
            internal IGuild[] GetUserGuilds(string userId)
            {
                List<string> guildsIdentifiers = userGuilds[userId];
                List<IGuild> guilds = new List<IGuild>();
                foreach (var guild in guildsIdentifiers)
                {
                    guilds.Add(this.guilds[guild]);
                }
                return guilds.ToArray();
            }
            public async Task Start()
            {
                await httpClient.StartAsync();
                GatewayInfo gatewayInfo = await httpClient.GetGatewayInfoAsync(); //TODO : GatewayUri должен содержать тип кодировки и версию API
                //HttpResponseMessage message = await httpClient.Get($"/api/channels/{511489105494802442}/messages/{742057322607673384}");
                //string res = ReadFromStream(await message.Content.ReadAsStreamAsync());
                await gatewayClient.StartAsync(gatewayInfo.Uri, "NTU5MDkwMTUzOTM1NjAxNjY1.XwSWOw.AD65cI1to13kYiOttDZx-cUEac0"); //TODO : перенести токен в конфиг и объеденить методы с API
                gatewayClient.DispatchEventHandler.GuildCreated += OnGuildCreated;
                gatewayClient.DispatchEventHandler.GuildUpdated += OnGuildUpdated;
                gatewayClient.DispatchEventHandler.GuildDeleted += OnGuildDeleted;
                gatewayClient.DispatchEventHandler.ChannelCreated += OnChannelCreated;
                gatewayClient.DispatchEventHandler.ChannelUpdated += OnChannelUpdated;
                gatewayClient.DispatchEventHandler.ChannelDeleted += OnChannelDeleted;
                gatewayClient.DispatchEventHandler.GuildRoleCreated += OnRoleCreated;
                gatewayClient.DispatchEventHandler.GuildRoleUpdated += OnRoleUpdated;
                gatewayClient.DispatchEventHandler.GuildRoleDeleted += OnRoleDeleted;
                gatewayClient.DispatchEventHandler.GuildMemberAdded += OnGuildUserAdded;
                gatewayClient.DispatchEventHandler.GuildMemberUpdated += OnGuildUserUpdated;
                gatewayClient.DispatchEventHandler.GuildMemberRemoved += OnGuildUserRemoved;
                gatewayClient.DispatchEventHandler.InviteCreated += OnInviteCreated;
                gatewayClient.DispatchEventHandler.InviteDeleted += OnInviteDeleted;
                gatewayClient.DispatchEventHandler.GuildBanAdded += OnUserBanned;
                gatewayClient.DispatchEventHandler.GuildBanRemoved += OnUserUnbanned;
                gatewayClient.DispatchEventHandler.MessageCreated += OnMessageReceived;
                gatewayClient.DispatchEventHandler.MessageDeleted += OnMessageDeleted;
                gatewayClient.DispatchEventHandler.MessageReactionAdded += OnMessageReactionAdded;
                gatewayClient.DispatchEventHandler.MessageReactionRemoved += OnMessageReactionRemoved;
                gatewayClient.DispatchEventHandler.MessageReactionRemovedAll += OnMessageAllReactionsRemoved;
                gatewayClient.DispatchEventHandler.MessageReactionEmojiRemoved += OnMessageEmojiRemoved;

                gatewayClient.DispatchEventHandler.Ready += OnReady;
                gatewayClient.SystemEventHandler.Connected += OnConnection;
            }
            public DiscordClient()
            {
                guilds = new Dictionary<string, IGuild>();
                httpClient = new DiscordHttpClient();
                gatewayClient = new DiscordGatewayClient();
            }
            #region Events handling
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
                    RaiseLog("New channel was created but no guild stored for this channel");
                else
                {
                    if (guilds[newChannel.GuildIdentifier] is Guild guild)
                    {
                        guild.AddChannel(newChannel);
                    }
                    else
                    {
                        RaiseLog("Cannot cast target IGuild to Guild");
                    }
                }
            }
            private void OnChannelUpdated(object sender, EventHandlerArgs args)
            {
                IGuildChannel newChannelInfo = args.EventData as IGuildChannel;
                if (!guilds.ContainsKey(newChannelInfo.GuildIdentifier))
                    RaiseLog("Handling ChannelUpdate event. Cannot find target guild");
                else
                {
                    if (guilds[newChannelInfo.GuildIdentifier] is Guild guild)
                    {
                        IChannel targetChannel = guild.TryToGetChannel(newChannelInfo.Identifier);
                        if (targetChannel is null)
                        {
                            RaiseLog("Handling ChannelUpdate event. Cannot find target channel");
                            return;
                        }
                        Console.WriteLine((targetChannel as IUpdatableChannel).UpdateChannel(newChannelInfo));
                    }
                    else
                    {
                        RaiseLog("Handling ChannelUpdate event. Cannot cast target IGuild to Guild");
                    }
                }
            }
            private void OnChannelDeleted(object sender, EventHandlerArgs args)
            {
                IGuildChannel channelToDelete = args.EventData as IGuildChannel;
                if (!guilds.ContainsKey(channelToDelete.GuildIdentifier))
                    RaiseLog("Handling ChannelDeleted event. Cannot find target guild");
                else
                {
                    if (guilds[channelToDelete.GuildIdentifier] is Guild guild)
                    {
                        guild.RemoveChannel(channelToDelete.Identifier);
                    }
                    else
                    {
                        RaiseLog("Handling ChannelUpdate event. Cannot cast target IGuild to Guild");
                    }
                }
            }
            private void OnRoleCreated(object sender, EventHandlerArgs args)
            {
                RoleEvent createdRole = args.EventData as RoleEvent;
                if (!guilds.ContainsKey(createdRole.GuildIdentifier))
                    RaiseLog("Handling RoleCreated event. Cannot find target guild");
                else
                {
                    if (guilds[createdRole.GuildIdentifier] is Guild guild)
                    {
                        guild.AddRole(createdRole.Role);
                    }
                    else
                    {
                        RaiseLog("Handling RoleCreated event. Cannot cast target IGuild to Guild");
                    }
                }
            }
            private void OnRoleUpdated(object sender, EventHandlerArgs args)
            {
                RoleEvent updatedRole = args.EventData as RoleEvent;
                if (!guilds.ContainsKey(updatedRole.GuildIdentifier))
                    RaiseLog("Handling RoleUpdated event. Cannot find target guild");
                else
                {
                    if (guilds[updatedRole.GuildIdentifier] is Guild guild)
                    {
                        Role roleToUpdate = guild.TryToGetRole(updatedRole.Role.Identifier);
                        if (roleToUpdate is null)
                        {
                            RaiseLog("Handling RoleUpdated event. Cannot find target role");
                        }
                        else
                        {
                            roleToUpdate.UpdateRole(updatedRole.Role);
                        }
                    }
                    else
                    {
                        RaiseLog("Handling RoleUpdated event. Cannot cast target IGuild to Guild");
                    }
                }
            }
            private void OnRoleDeleted(object sender, EventHandlerArgs args)
            {
                RoleDeletedEvent deletedRole = args.EventData as RoleDeletedEvent;
                if (!guilds.ContainsKey(deletedRole.GuildIdentifier))
                    RaiseLog("Handling RoleDeleted event. Cannot find target guild");
                else
                {
                    if (guilds[deletedRole.GuildIdentifier] is Guild guild)
                    {
                        guild.RemoveRole(deletedRole.RoleIdentifier);
                    }
                    else
                    {
                        RaiseLog("Handling RoleDeleted event. Cannot cast target IGuild to Guild");
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
                        RaiseLog("Handling GuildUserAdded event. Cannot cast target IGuild to Guild");
                    }
                }
                else
                {
                    RaiseLog("Handling GuildUserAdded event. Cannot cast target IGuild to Guild");
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
                        RaiseLog("Handling GuildUserUpdated event. Cannot cast target IGuild to Guild");
                    }
                }
                else
                {
                    RaiseLog("Handling RoleDeleted event. Cannot find target guild");
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
                        RaiseLog("Handling GuildUserUpdated event. Cannot cast target IGuild to Guild");
                    }
                }
                else
                {
                    RaiseLog("Handling UserDeleted event. Cannot cast target IGuild to Guild");
                }
            }
            private void OnInviteCreated(object sender, EventHandlerArgs args)
            {
                IInvite newInvite = args.EventData as IInvite;
                if (TryToGetGuild(newInvite.GuildIdentifier) is Guild guild)
                    guild.AddInvite(newInvite);
                else
                    RaiseLog("Error during InviteCreated event handling. Cannot find target guild or cast it to Guild");
            }
            private void OnInviteDeleted(object sender, EventHandlerArgs args)
            {
                IInvite deletedInvite = args.EventData as IInvite;
                if (TryToGetGuild(deletedInvite.Guild.Identifier) is Guild guild)
                    guild.RemoveInvite(deletedInvite.Code);
                else
                    RaiseLog("Error during InviteDeleted event handling. Cannot find target guild or cast it to Guild");
            }
            private void OnUserBanned(object sender, EventHandlerArgs args)
            {
                Ban bannedUser = args.EventData as Ban;
                if (TryToGetGuild(bannedUser.GuildIdentifier) is Guild guild)
                    guild.AddBan(bannedUser);
                else
                    RaiseLog("Error during UserBanned event handling. Cannot find target guild or cast it to Guild");
            }
            private void OnUserUnbanned(object sender, EventHandlerArgs args)
            {
                Ban bannedUser = args.EventData as Ban;
                if (TryToGetGuild(bannedUser.GuildIdentifier) is Guild guild)
                    guild.RemoveBan(bannedUser.User.Identifier);
                else
                    RaiseLog("Error during UserUnbanned event handling. Cannot find target guild or cast it to Guild");
            }
            private void OnMessageReceived(object sender, EventHandlerArgs args)
            {
                IMessage newMessage = args.EventData as IMessage;
                if (TryToGetGuild((newMessage.Channel as IGuildChannel).GuildIdentifier) is Guild guild)
                    if (guild.TryToGetChannel(newMessage.Channel.Identifier) is IMessageEditableChannel channel)
                        channel.AddMessage(newMessage);
                    else
                        RaiseLog("Error during MessageReceived event handling. Cannot find target channel or cast it to IMessageEditableChannel");
                else
                    RaiseLog("Error during MessageReceived event handling. Cannot find target guild or cast it to Guild");
            }
            private void OnMessageDeleted(object sender, EventHandlerArgs args)
            {
                IMessage deletedMessage = args.EventData as IMessage;
                if (TryToGetGuild((deletedMessage.Channel as IGuildChannel).GuildIdentifier) is Guild guild)
                    if (guild.TryToGetChannel(deletedMessage.Channel.Identifier) is ITextChannel channel)
                        channel.RemoveMessage(deletedMessage.Identifier);
                    else
                        RaiseLog("Error during MessageDeleted event handling. Cannot find target channel or cast it to ITextChannel");
                else
                    RaiseLog("Error during MessageReceived event handling. Cannot find target guild or cast it to Guild");
            }
            private void OnMessageReactionAdded(object sender, EventHandlerArgs args)
            {
                MessageReactionEvent newReaction = args.EventData as MessageReactionEvent;
                if (TryToGetGuild(newReaction.guildIdentifier) is Guild guild)
                    if (guild.TryToGetChannel(newReaction.channelIdentifier) is ITextChannel channel)
                    {
                        if (channel.TryToGetMessage(newReaction.MessageIdentifier) is IUpdatableMessage message)
                        {
                            message.AddReaction(newReaction.Emoji);
                        }
                        else
                            RaiseLog("Error during MessageReactionAdded event handling. Cannot find target message or cast it to IUpdatableMessage");
                    }
                    else
                        RaiseLog("Error during MessageReactionAdded event handling. Cannot find target channel or cast it to ITextChannel");
                else
                    RaiseLog("Error during MessageReactionAdded event handling. Cannot find target guild or cast it to Guild");
            }
            private void OnMessageReactionRemoved(object sender, EventHandlerArgs args)
            {
                if (args.EventData is MessageReactionEvent newReaction)
                {
                    // TODO: пробрасывать
                }
                else
                {
                    RaiseLog("Error during MessageReactionAdded event handling. Cannot cast received data to MessageReactionEvent");
                }
            }
            private void OnMessageAllReactionsRemoved(object sender, EventHandlerArgs args)
            {
                if (args.EventData is MessageReactionEvent newReaction)
                {
                    // TODO: пробрасывать
                }
                else
                {
                    RaiseLog("Error during MessageReactionAdded event handling. Cannot cast received data to MessageReactionEvent");
                }
            }
            private void OnMessageEmojiRemoved(object sender, EventHandlerArgs args)
            {
                if (args.EventData is MessageReactionEvent newReaction)
                {
                    // TODO: пробрасывать
                }
                else
                {
                    RaiseLog("Error during MessageReactionAdded event handling. Cannot cast received data to MessageReactionEvent");
                }
            }
            private async void OnGuildCreated(object sender, EventHandlerArgs args)
            {
                IGuild guild = args.EventData as IGuild;
                if (guilds.ContainsKey(guild.Identifier))
                    guilds[guild.Identifier] = guild;
                else
                    guilds.Add(guild.Identifier, guild);
                GuildCreated(sender, args);
                //Guild guild = arg.EventData as Guild;
                //List<Ban> bans = await GetGuildBannedUsers(guild.Identifier);
                //List<Invite> invites = await GetGuildInvites(guild.Identifier);
                ////TAI : не добавлять каждый инвайт\бан в список, а заменять список 
                ////т.к. эти запросы происходят только при первичном создании гильдии
                //for (int i = 0; i < invites.Count; i++)
                //{
                //    guild.AddInvite(invites[i]);
                //}
                //for(int i = 0; i < bans.Count; i++)
                //{
                //    //TODO : уставливать здесь в бане GuildID(в данном запросе приходит без инфы о гильдии)
                //    guild.AddBan(bans[i]);
                //}
            }
            private void OnGuildUpdated(object sender, EventHandlerArgs args)
            {
                Guild guild = args.EventData as Guild;
                (guilds[guild.Identifier] as Guild).UpdateGuild(guild);
            }
            private void OnReady(object sender, EventHandlerArgs args)
            {
                Ready ready = args.EventData as Ready;
                BotUser = ready.User;
                foreach (var guild in ready.Guilds)
                    guilds.Add(guild.Identifier, guild as IGuild);
                readyReceived = DateTime.Now;
            }
            private void OnConnection(IGatewayDataObject payload)
            {
                Console.WriteLine("Connected");//TODO : do smth
            }
            #endregion
            private async Task<List<Ban>> GetGuildBannedUsers(string guildId)
            {
                string endPoint = $"/api/guilds/{guildId}/bans";
                HttpResponseMessage requestResult = await httpClient.Get(endPoint);
                string content = ReadFromStream(await requestResult.Content.ReadAsStreamAsync());
                return JsonConvert.DeserializeObject<List<Ban>>(content);
            }
            private async Task<List<Invite>> GetGuildInvites(string guildId)
            {
                string endPoint = $"/api/guilds/{guildId}/invites";
                HttpResponseMessage requestResult = await httpClient.Get(endPoint);
                string content = ReadFromStream(await requestResult.Content.ReadAsStreamAsync());
                return JsonConvert.DeserializeObject<List<Invite>>(content);
            }
            private string ReadFromStream(Stream stream)
            {
                using (stream)
                    return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}