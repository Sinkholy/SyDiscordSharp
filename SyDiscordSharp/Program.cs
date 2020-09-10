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
                gatewayClient.DispatchEventHandler.MessageDeletedBulk += OnMessageDeletedBulk;
                gatewayClient.DispatchEventHandler.MessageReactionAdded += OnMessageReactionAdded;
                gatewayClient.DispatchEventHandler.MessageReactionRemoved += OnMessageReactionRemoved;
                gatewayClient.DispatchEventHandler.MessageReactionRemovedAll += OnMessageAllReactionsRemoved;
                gatewayClient.DispatchEventHandler.MessageReactionEmojiRemoved += OnMessageEmojiRemoved;
                gatewayClient.DispatchEventHandler.VoiceStateUpdated += OnVoiceStateUpdated;
                gatewayClient.DispatchEventHandler.VoiceServerUpdated += OnVoiceServerUpdated;
                gatewayClient.DispatchEventHandler.TypingStarted += OnTypingStarted;
                gatewayClient.DispatchEventHandler.GuildIntegrationsUpdated += OnGuildIntegrationUpdated;
                gatewayClient.DispatchEventHandler.WebhooksUpdated += OnWebhookUpdated;
                gatewayClient.DispatchEventHandler.ChannelPinsUpdated += OnChannelPinsUpdated;
                gatewayClient.DispatchEventHandler.GuildEmojisUpdated += OnGuildEmojisUpdated;
                gatewayClient.DispatchEventHandler.PresenceUpdated += OnPresenceUpdated;

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
            private async void OnGuildCreated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IGuild guild)
                {
                    //guild.BannedUsersList = await GetGuildBannedUsers(guild.Identifier);
                    //List<Invite> invites = await GetGuildInvites(guild.Identifier);
                    //guild.InvitesList = invites.Select(x => x as IInvite).ToList();
                    guilds.Add(guild.Identifier, guild);
                }
                else
                {
                    RaiseLog("Handling GuildCreated event. Cannot cast received data to IGuild.");
                }
            }
            private void OnGuildUpdated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IGuild newGuildInfo)
                {
                    if (TryToGetGuild(newGuildInfo.Identifier) is Guild oldGuildInfo)
                    {
                        (newGuildInfo as IUpdatableGuild).LoadInfoFromOldGuild(oldGuildInfo);
                        guilds[oldGuildInfo.Identifier] = newGuildInfo;
                    }
                    else
                    {
                        RaiseLog("Handling GuildUpdated event. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Handling GuildUpdated event. Cannot cast received data to IGuild.");
                }
            }
            private void OnGuildDeleted(object sender, EventHandlerArgs args) // TODO: проверять был ли пользователь удален из гильдии
            {
                if (args.EventData is GuildPreview deletedGuild)
                {
                    if (deletedGuild.Unavailable)
                    {
                        if (guilds.ContainsKey(deletedGuild.Identifier))
                        {
                            guilds.Remove(deletedGuild.Identifier);
                        }
                        else
                        {
                            RaiseLog("Handling GuildDeleted event. Cannot find target guild.");
                        }
                    }
                    else
                    {
                        guilds.Remove(deletedGuild.Identifier);
                    }
                }
                else
                {
                    RaiseLog("Handling GuildDeleted event. Cannot cast received data to GuildPreview.");
                }
            }
            private void OnChannelCreated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IGuildChannel newChannel)
                {
                    if (TryToGetGuild(newChannel.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.AddChannel(newChannel);
                    }
                    else
                    {
                        RaiseLog("Handling ChannelCreated event. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Handling ChannelCreated event. Cannot cast received data to IGuildChannel.");
                }
            }
            private void OnChannelUpdated(object sender, EventHandlerArgs args) // TODO: проверить все IUpdatable интерфейсы и убрать 
                                                                                // если они неактуальны более.
            {
                if (args.EventData is IGuildChannel newChannelInfo)
                {
                    IChannel prevChannelInfo;
                    if (TryToGetGuild(newChannelInfo.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        prevChannelInfo = guild.OverrideChannel(newChannelInfo);
                    }
                    else
                    {
                        RaiseLog("Handling ChannelUpdated event. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Handling ChannelUpdated event. Cannot cast received data to IGuildChannel.");
                }
            }
            private void OnChannelDeleted(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IGuildChannel channelToDelete)
                {
                    if (TryToGetGuild(channelToDelete.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.RemoveChannel(channelToDelete.Identifier);
                    }
                    else
                    {
                        RaiseLog("Handling ChannelDeleted event. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Handling ChannelDeleted event. Cannot cast received data to IGuildChannel.");
                }
            }
            private void OnRoleCreated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is RoleEvent createdRole)
                {
                    if (TryToGetGuild(createdRole.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.AddRole(createdRole.Role);
                    }
                    else
                    {
                        RaiseLog("Handling RoleCreated event. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Handling RoleCreated event. Cannot cast received data to RoleEvent.");
                }
            }
            private void OnRoleUpdated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is RoleEvent updatedRole)
                {
                    if (TryToGetGuild(updatedRole.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.OverrideRole(updatedRole.Role);
                    }
                    else
                    {
                        RaiseLog("Handling RoleUpdated event. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Handling RoleUpdated event. Cannot cast received data to RoleEvent.");
                }
            }
            private void OnRoleDeleted(object sender, EventHandlerArgs args)
            {
                if (args.EventData is RoleDeletedEvent deletedRole)
                {
                    if (TryToGetGuild(deletedRole.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.RemoveRole(deletedRole.RoleIdentifier);
                    }
                    else
                    {
                        RaiseLog("Handling RoleDeleted event. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Handling RoleDeleted event. Cannot cast received data to RoleDeletedEvent.");
                }
            }
            private void OnGuildUserAdded(object sender, EventHandlerArgs args)
            {
                if (args.EventData is GuildUser newUser) // TODO: IGuildUser.
                {
                    if (TryToGetGuild(newUser.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.AddUser(newUser);
                    }
                    else
                    {
                        RaiseLog("Handling GuildUserAdded event. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Handling GuildUserAdded event. Cannot cast received data to GuildUser");
                }
            }
            private void OnGuildUserUpdated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is GuildUser newUserInfo) // TODO: у юзера пресенсы и роли не могут быть высчитаны
                                                             // TODO: IGuildUser
                {
                    GuildUser prevUserInfo = null;
                    if (TryToGetGuild(newUserInfo.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        prevUserInfo = guild.OverrideGuildUser(newUserInfo);
                    }
                    else
                    {
                        RaiseLog("Handling GuildMemberUpdated event. Cannot find target guild.");
                    }

                    // НАРАБОТКИ ДЛЯ ЛОГЕРА
                    //List<EmbedField> fields = new List<EmbedField> 
                    //{ 
                    //    new EmbedField(EmbedField.WhiteSpace, EmbedField.WhiteSpace, true), 
                    //    new EmbedField(EmbedField.WhiteSpace, "> **Old**", true), 
                    //    new EmbedField(EmbedField.WhiteSpace, "> **New**", true), 
                    //};
                    //if(prevUserInfo.Nickname != newUserInfo.Nickname)
                    //{
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, "\n**Nickname**", true));
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```{(prevUserInfo.Nickname is null ? prevUserInfo.Username : prevUserInfo.Nickname)}```", true));
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```{(newUserInfo.Nickname is null ? newUserInfo.Username : newUserInfo.Nickname)}```", true));

                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, "\u000d**Username**", true));
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```{prevUserInfo.Username}```", true));
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```{newUserInfo.Username}```", true));

                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, "**Discriminator**", true));
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```#{prevUserInfo.Discriminator}```", true));
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```#{newUserInfo.Discriminator}```", true));

                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, "\n**eMail**", true));
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```Sobaka@Sobaka.ru```", true));
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```NewSobaka@Sobaka.ru```", true));

                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, "\n**Roles**", true));
                    //     fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```Альфа \nБета \nГамма \nЭпсилон```", true));
                    //    fields.Add(new EmbedField(EmbedField.WhiteSpace, $"```Альфа \nБета```", true));
                    //}
                    //// TODO: вынести генерирование ссылок в метод или даже в самого юзера (гениально)
                    //EmbedImage embedImage = new EmbedImage("https://"+$"cdn.discordapp.com/avatars/{newUserInfo.Identifier}/{newUserInfo.AvatarIdentifier}.png", null, 128, 128);
                    //EmbedFooter footer = new EmbedFooter($"User id: {newUserInfo.Identifier}", "https://" + $"cdn.discordapp.com/avatars/{newUserInfo.Identifier}/{newUserInfo.AvatarIdentifier}.png", null);
                    //EmbedAuthor author = new EmbedAuthor("SyDiscordSharp", "https://vk.com/sinkholy", "https://cdn.discordapp.com/embed/avatars/0.png", null);

                    //EmbedData embedData = new EmbedData(null,
                    //                                    $"{newUserInfo.Mention} updated",
                    //                                    null,
                    //                                    0000000,
                    //                                    DateTime.Now,
                    //                                    footer,
                    //                                    null,
                    //                                    embedImage,
                    //                                    null,
                    //                                    author,
                    //                                    null,
                    //                                    fields.ToArray());
                    //EmbeddedMessage messageToSend = new EmbeddedMessage(null,
                    //                                                    null,
                    //                                                    new EmbedData[] { embedData });
                    //IChannel targetChannel = (TryToGetGuild("540324745367781376")).TryToGetChannel("543127619936190493");
                    //SendMessage(targetChannel as ITextChannel, messageToSend);




                }
                else
                {
                    RaiseLog("Handling GuildMemberUpdated event. Cannot cast received data to GuildUser.");
                }
            }
            private void OnGuildUserRemoved(object sender, EventHandlerArgs args)
            {
                if (args.EventData is GuildMember deletedUser) // TODO: IGuildUser
                {
                    if (TryToGetGuild(deletedUser.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.RemoveUser(deletedUser.User.Identifier);
                    }
                    else
                    {
                        RaiseLog("Handling UserDeleted event. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Handling UserDeleted event. Cannot cast received data to GuildMember.");
                }
            }
            private void OnGuildEmojisUpdated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is GuildEmojiUpdatedEvent emojisData)
                {
                    if (TryToGetGuild(emojisData.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.SetNewGuildEmojis(emojisData.Emojis);
                        // TODO: прокидывание события
                    }
                    else
                    {
                        RaiseLog("Error during GuildEmojisUpdated event handling. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Error during GuildEmojisUpdated event handling. Cannot cast received data to GuildEmojiUpdatedEvent.");
                }
            }
            private void OnInviteCreated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IInvite newInvite)
                {
                    if (TryToGetGuild(newInvite.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.AddInvite(newInvite);
                    }
                    else
                    {
                        RaiseLog("Error during InviteCreated event handling. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Error during InviteCreated event handling. Cannot cast received data to IInvite.");
                }
            }
            private void OnInviteDeleted(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IInvite deletedInvite)
                {
                    if (TryToGetGuild(deletedInvite.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.RemoveInvite(deletedInvite.Code);
                    }
                    else
                    {
                        RaiseLog("Error during InviteDeleted event handling. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Error during InviteDeleted event handling. Cannot cast received data to IInvite.");
                }
            }
            private void OnUserBanned(object sender, EventHandlerArgs args)
            {
                if (args.EventData is Ban bannedUser)
                {
                    if (TryToGetGuild(bannedUser.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.AddBan(bannedUser);
                    }
                    else
                    {
                        RaiseLog("Error during UserBanned event handling. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Error during UserBanned event handling. Cannot cast received data to Ban.");
                }
            }
            private void OnUserUnbanned(object sender, EventHandlerArgs args)
            {
                if (args.EventData is Ban bannedUser)
                {
                    if (TryToGetGuild(bannedUser.GuildIdentifier) is IUpdatableGuild guild)
                    {
                        guild.RemoveBan(bannedUser.User.Identifier);
                    }
                    else
                    {
                        RaiseLog("Error during UserUnbanned event handling. Cannot find target guild.");
                    }
                }
                else
                {
                    RaiseLog("Error during UserUnbanned event handling. Cannot cast received data to Ban.");
                }
            }
            private void OnMessageReceived(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IMessage newMessage)
                {
                    if (newMessage.MentionedUsers.Where(x => x.Identifier == BotUser.Identifier).SingleOrDefault() != null)
                    {
                        // Точка входа при упоминании
                    }
                    // TODO: прокидывание
                }
                else
                {
                    RaiseLog("Error during MessageReceived event handling. Cannot cast received data to Message.");
                }
            }
            private void OnMessageDeleted(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IMessage deletedMessage)
                {
                    // TODO: прокидывание сообщения
                }
                else
                {
                    RaiseLog("Error during MessageReceived event handling. Cannot cast received data to IMessage");
                }
            }
            private void OnMessageDeletedBulk(object sender, EventHandlerArgs args)
            {
                if (args.EventData is MessageDeletedBulk deletedMessages)
                {
                    // TODO: прокидывание сообщения
                }
                else
                {
                    RaiseLog("Error during MessageReceived event handling. Cannot cast received data to MessageBase[]");
                }
            }
            private void OnMessageReactionAdded(object sender, EventHandlerArgs args)
            {
                if (args.EventData is MessageReactionEvent newReaction)
                {
                    // TODO: пробрасывать
                }
                else
                {
                    RaiseLog("Error during MessageReactionAdded event handling. Cannot cast received data to MessageReactionEvent.");
                }
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
            private void OnVoiceStateUpdated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IVoiceSession newVoiceState)
                {
                    if (TryToGetGuild(newVoiceState.GuildIdentifier) is IGuild guild) // TODO: GuildIdentifier отмечен как ? 
                                                                                      // надо продумать механизм получения
                                                                                      // канала как-то вне гильдии
                    {
                        IVoiceSession previousSessionState;
                        if (newVoiceState.ChannelIdentifier != null)
                        {
                            if (guild.TryToGetVoiceSession(newVoiceState.SessionIdentifier) is IVoiceSession)
                            {
                                if (guild is IUpdatableGuild updatableGuild)
                                {
                                    previousSessionState = updatableGuild.OverrideVoiceSession(newVoiceState);
                                }
                                if (guild.TryToGetUser(newVoiceState.UserIdentifier) is IUpdatableGuildUser updatableUser)
                                {
                                    updatableUser.UpdateMutedState(newVoiceState.Muted);
                                    updatableUser.UpdateDeafenedState(newVoiceState.Deafened);
                                    updatableUser.UpdateSelfDeafenedState(newVoiceState.SelfDeafened);
                                    updatableUser.UpdateSelfMutedState(newVoiceState.SelfMuted);
                                    updatableUser.UpdateSelfStreamState(newVoiceState.SelfStream);
                                    updatableUser.UpdateSelfVideoState(newVoiceState.SelfVideo);
                                }
                            }
                            else
                            {
                                if (guild is IUpdatableGuild updatableGuild)
                                {
                                    updatableGuild.AddVoiceSession(newVoiceState);
                                }
                            }
                        }
                        else
                        {
                            if (guild is IUpdatableGuild updatableGuild)
                            {
                                previousSessionState = updatableGuild.RemoveVoiceSession(newVoiceState.SessionIdentifier);
                            }
                        }

                        // ЭТО НАБРОСКИ ДЛЯ ЛОГЕРА
                        //IGuild targetGuild = TryToGetGuild(newVoiceState.GuildIdentifier);
                        //IUser targetUser = targetGuild?.TryToGetUser(newVoiceState.UserIdentifier);

                        //string action = string.Empty;
                        //IVoiceChannel channel;
                        //ChannelCategory category;
                        //if (prevState is null)
                        //{
                        //    channel = targetGuild?.TryToGetChannel(newVoiceState.ChannelIdentifier) as IVoiceChannel;
                        //    category = targetGuild?.TryToGetChannel(channel.ParentIdentifier) as ChannelCategory;
                        //    action = $"Joined channel: {category.Name}/{channel.Name}";
                        //}
                        //else if (newVoiceState.ChannelIdentifier == null)
                        //{
                        //    channel = targetGuild?.TryToGetChannel(prevState.ChannelIdentifier) as IVoiceChannel;
                        //    category = targetGuild?.TryToGetChannel(channel.ParentIdentifier) as ChannelCategory;
                        //    action = $"Left channel:  {category.Name}/{channel.Name}";
                        //}
                        //else
                        //{
                        //    if (prevState.ChannelIdentifier != newVoiceState.ChannelIdentifier)
                        //    {
                        //        channel = targetGuild?.TryToGetChannel(newVoiceState.ChannelIdentifier) as IVoiceChannel;
                        //        category = targetGuild?.TryToGetChannel(channel.ParentIdentifier) as ChannelCategory;
                        //        action = $"Moved to: {category.Name}/{channel.Name}";
                        //    }
                        //    else if (prevState.Deafened != newVoiceState.Deafened)
                        //    {
                        //        action = newVoiceState.Deafened ? "Server deafened" : "Server undeafened";
                        //    }
                        //    else if (prevState.Muted != newVoiceState.Muted)
                        //    {
                        //        action = newVoiceState.Muted ? "Server muted" : "Server unmuted";
                        //    }
                        //    else if (prevState.SelfDeafened != newVoiceState.SelfDeafened)
                        //    {
                        //        action = newVoiceState.SelfDeafened ? "Self deafened" : "Self undeafened";
                        //    }
                        //    else if (prevState.SelfMuted != newVoiceState.SelfMuted)
                        //    {
                        //        action = newVoiceState.SelfMuted ? "Self muted" : "Self unmuted";
                        //    }
                        //    else if (prevState.SelfVideo != newVoiceState.SelfVideo)
                        //    {
                        //        action = newVoiceState.SelfVideo ? "Started video" : "Closed video";
                        //    }
                        //    else if (prevState.SelfStream != newVoiceState.SelfStream)
                        //    {
                        //        action = newVoiceState.SelfStream ? "Started stream" : "Closed stream";
                        //    }
                        //}
                        //string messageContent = $"User: {targetUser.Mention} \n" +
                        //                        $"{action}";

                        //IMessage messageToLog = new Message(messageContent, string.Empty);

                        //ITextChannel loggingChannel = targetGuild?.TryToGetChannel("543127619936190493") as ITextChannel;
                        //SendMessage(loggingChannel, messageToLog);
                    }
                    else
                    {
                        RaiseLog("Error during MessageReactionAdded event handling. Cannot find target guild or cast it to Guild");
                    }
                }
                else
                {
                    RaiseLog("Error during MessageReactionAdded event handling. Cannot cast received data to MessageReactionEvent");
                }
            }
            private void OnPresenceUpdated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is IPresence presenceReceived)
                {
                    if (TryToGetGuild(presenceReceived.GuildIdentifier) is IGuild guild)
                    {
                        if (presenceReceived.UserStatus == UserStatus.Offline)
                        {
                            (guild as IUpdatableGuild).RemovePresence(presenceReceived.UserIdentifier);
                        }
                        else
                        {
                            if (guild.TryToGetPresence(presenceReceived.UserIdentifier) is IPresence)
                            {
                                (guild as IUpdatableGuild).OverridePresence(presenceReceived);
                            }
                            else
                            {
                                (guild as IUpdatableGuild).AddPresence(presenceReceived);
                            }
                        }
                    }
                    else
                    {
                        RaiseLog("Error during PresenceUpdated event handling. Cannot find target Guild.");
                    }
                }
                else
                {
                    RaiseLog("Error during PresenceUpdated event handling. Cannot cast received data to PresenceUpdatedEvent.");
                }
            }
            private void OnVoiceServerUpdated(object sender, EventHandlerArgs args) // TODO: не смог получить данное событие
            {
                if (args.EventData is VoiceServerUpdate newServerInfo)
                {

                }
                else
                {
                    RaiseLog("Error during MessageReactionAdded event handling. Cannot cast received data to VoiceServerUpdate");
                }
            }
            private void OnTypingStarted(object sender, EventHandlerArgs args)
            {
                // TODO: пробрасывать событие наверх
            }
            private void OnGuildIntegrationUpdated(object sender, EventHandlerArgs args)
            {
                // TODO: пробрасывание события
            }
            private void OnWebhookUpdated(object sender, EventHandlerArgs args)
            {
                // TODO: пробрасывание события
            }
            private void OnChannelPinsUpdated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is ChannelPinsUpdatedEvent pinInfo)
                {
                    // TODO: пробрасывание события
                }
                else
                {
                    RaiseLog("Error duting ChannelPinsUpdated event handling. Cannot cast received data to ChannelPinsUpdatedEvent.");
                }
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