﻿using API;
using Gateway;
using Gateway.Entities;
using Gateway.Entities.Channels;
using Gateway.Entities.Channels.Guild;
using Gateway.Entities.Channels.Guild.IUpdatable;
using Gateway.Entities.Embed;
using Gateway.Entities.Emojis;
using Gateway.Entities.Guilds;
using Gateway.Entities.Invite;
using Gateway.Entities.Message;
using Gateway.Entities.Presences;
using Gateway.Entities.Users;
using Gateway.Entities.VoiceSession;
using Gateway.Payload.DataObjects;
using Gateway.Payload.DataObjects.Dispatch.DispatchEvents;
using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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
            public TimeSpan Uptime => DateTime.Now - readyReceived; // TODO: считать тотальный аптайм, а не сессионный
            private Dictionary<string, IGuild> guilds;
            private Dictionary<string, List<string>> userGuilds;
            private readonly DiscordHttpClient httpClient;
            private readonly DiscordGatewayClient gatewayClient;
            private DateTime readyReceived;
            #endregion
            #region Singleton
            public static DiscordClient GetInstance() // TAI : потокобезопасность
                                                      // TODO : записывать в Instance - this, чтобы не было конфликтов. 
            {
                if (instance == null)
                {
                    instance = new DiscordClient();
                }
                return instance;
            }
            private static DiscordClient instance;
            #endregion

            internal IGuild TryToGetGuild(string id)
            {
                if (guilds.TryGetValue(id, out IGuild guild))
                {
                    return guild;
                }
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
                await gatewayClient.StartAsync(gatewayInfo.Uri, "NTU5MDkwMTUzOTM1NjAxNjY1.XJaDSA.IX8ZHPTebYrgzYPJsXyezjA40EQ"); //TODO : перенести токен в конфиг и объеденить методы с API
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
                Log += x => Console.WriteLine(x);
            }
            public DiscordClient()
            {
                httpClient = new DiscordHttpClient();
                gatewayClient = new DiscordGatewayClient();
            }
            #region Gateway events handling
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
                        IUpdatableGuildTextChannel channel = TryToGetGuild("540324745367781376").TryToGetChannel("543127619936190493") as IUpdatableGuildTextChannel;
                        channel.SetNewName("#2");
                        channel.SetNewTopic("Новый топик в #2");
                        channel.SetNewCategory("543127580413394973");


                        ModifyChannel(channel as IChannel);
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
            private void OnReady(object sender, EventHandlerArgs args)
            {
                Ready ready = args.EventData as Ready;
                BotUser = ready.User;
                guilds = new Dictionary<string, IGuild>(capacity: ready.Guilds.Length);
                readyReceived = DateTime.Now;
            }
            private void OnConnection(IGatewayDataObject payload)
            {
                Console.WriteLine("Connected");//TODO : do smth
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
            private void OnUserUpdated(object sender, EventHandlerArgs args)
            {
                if (args.EventData is User newUserInfo)
                { // TODO: закончить
                    //(BotUser as IUpdatableUser).UpdateAvatar(newUserInfo.AvatarIdentifier);
                    //(BotUser as IUpdatableUser).UpdateUsername(newUserInfo.Username);
                }
                else
                {
                    RaiseLog("Error duting UserUpdated event handling. Cannot cast received data to User.");
                }
            }
            #endregion
            #region Http methods
            private async Task ModifyBotUser()
            {
                string endPoint = "/api/users/@me";
                NewBotUserInfo newUserInfo = new NewBotUserInfo { username = "newBotNameTest" };
                string msgToSend = JsonConvert.SerializeObject(newUserInfo, typeof(NewBotUserInfo), null);
                StringContent contentToSend = new StringContent(msgToSend, Encoding.UTF8, "application/json");
                HttpResponseMessage requestResult = await httpClient.Patch(endPoint, contentToSend);
            }
            private async Task ModifyChannel(IChannel channel)
            {
                string endPoint = $"/api/channels/{channel.Identifier}";
                //ModifyChanelRequest request = new ModifyChanelRequest { Name = "SomeShit", Type = ChannelType.GuildCategory };
                StringContent content = new StringContent(JsonConvert.SerializeObject(channel), Encoding.UTF8, "application/json");
                var response = await httpClient.Patch(endPoint, content);
                string responseString = await response.Content.ReadAsStringAsync();
            }
            private async Task DeleteChannel(IChannel channel)
            {
                await DeleteChannel(channel.Identifier);
            }
            private async Task DeleteChannel(string channelId)
            {
                string endPoint = $"/api/channels/{channelId}";
                var response = await httpClient.Delete(endPoint);
                string responseString = await response.Content.ReadAsStringAsync();
            }

            internal class ModifyChanelRequest
            {
                [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
                internal string Name;
                [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
                internal ChannelType Type;
                [JsonProperty(PropertyName = "position", NullValueHandling = NullValueHandling.Ignore)]
                internal int? Position;
                [JsonProperty(PropertyName = "topic", NullValueHandling = NullValueHandling.Ignore)]
                internal string Topic;
                [JsonProperty(PropertyName = "nsfw", NullValueHandling = NullValueHandling.Ignore)]
                internal bool? NSFW;
                [JsonProperty(PropertyName = "user_limit", NullValueHandling = NullValueHandling.Ignore)]
                internal int? UserLimit;
                [JsonProperty(PropertyName = "bitrate", NullValueHandling = NullValueHandling.Ignore)]
                internal int? Bitrate;
                [JsonProperty(PropertyName = "rate_limit_per_user", NullValueHandling = NullValueHandling.Ignore)]
                internal int? UserRateLimit;
                [JsonProperty(PropertyName = "permission_overwrites", NullValueHandling = NullValueHandling.Ignore)]
                internal List<PermissionOverwrite> PermissionOverwrites;
                [JsonProperty(PropertyName = "parent_id", NullValueHandling = NullValueHandling.Ignore)]
                internal string CategoryIdentifier;
            }
            [JsonObject(MemberSerialization.OptIn)]
            internal class NewBotUserInfo
            {
                [JsonProperty(PropertyName = "username", NullValueHandling = NullValueHandling.Ignore)]
                internal string username;
                [JsonProperty(PropertyName = "avatar", NullValueHandling = NullValueHandling.Ignore)]
                internal string avatar;
            }
            private async Task BulkDeleteMessages(string channelId, ICollection<string> identifiers)
            {
                string endPoint = $"/api/channels/{channelId}/messages/bulk-delete";
                MsgesToDelete message = new MsgesToDelete
                {
                    messages = identifiers
                };
                string msgToSend = JsonConvert.SerializeObject(message, typeof(MsgesToDelete), null);
                StringContent contentToSend = new StringContent(msgToSend, Encoding.UTF8, "application/json");
                HttpResponseMessage requestResult = await httpClient.Post(endPoint, contentToSend);

            }
            [JsonObject(MemberSerialization.OptIn)]
            internal class MsgesToDelete
            {
                [JsonProperty(PropertyName = "messages")]
                internal ICollection<string> messages; // up to 2000 chars
            }
            public async Task AddReactionToMessage(IEmoji emoji, string channelId, string messageId)
            {
                string endPoint = $"/api/channels/{channelId}/messages/{messageId}/reactions/{emoji.UrlEncoded}/@me";
                HttpResponseMessage requestResult = await httpClient.Put(endPoint);
            }
            public async Task AddReactionToMessage(IEmoji emoji, ITextChannel channel, string messageId)
                => await AddReactionToMessage(emoji, channel.Identifier, messageId);
            private async void SendMessage(ITextChannel channel, IMessage message)
            {
                string endPoint = $"/api/channels/{channel.Identifier}/messages";
                string msgToSend = JsonConvert.SerializeObject(new MsgClass
                {
                    tts = message.TTS,
                    nonce = message.Nonce,
                    content = message.Content,
                    Embed = (message as EmbeddedMessage)?.Embeds[0], // TAI: пахнет писькой
                });
                StringContent content = new StringContent(msgToSend, Encoding.UTF8, "application/json");
                HttpResponseMessage requestResult = await httpClient.Post(endPoint, content);
            }
            private async void SendMessageWithAttachement(ITextChannel channel,
                                                          IMessage message,
                                                          byte[] fileData,
                                                          string fileName)
            {
                string endPoint = $"/api/channels/{channel.Identifier}/messages";
                string msgToSend = JsonConvert.SerializeObject(new MsgClass
                {
                    tts = message.TTS,
                    nonce = message.Nonce,
                    content = message.Content,
                    Embed = (message as EmbeddedMessage)?.Embeds[0], // TAI: пахнет писькой
                });
                StringContent stringContent = new StringContent(msgToSend);
                ByteArrayContent fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = fileName
                };
                stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "payload_json"
                };
                MultipartContent multipartContent = new MultipartContent("form-data")
                {
                    stringContent,
                    fileContent
                };
                HttpResponseMessage requestResult = await httpClient.Post(endPoint, multipartContent);
            }
            [JsonObject(MemberSerialization.OptIn)]
            internal class MsgClass
            {
                [JsonProperty(PropertyName = "content")]
                internal object content; // up to 2000 chars
                [JsonProperty(PropertyName = "tts")]
                internal bool tts;
                [JsonProperty(PropertyName = "nonce")]
                internal string nonce;
                [JsonProperty(PropertyName = "embed")]
                internal EmbedData Embed;
            }
            private async Task<IMessage> GetMessage(string channelId, string messageId)
            {
                string endPoint = $"/api/channels/{channelId}/messages/{messageId}";
                HttpResponseMessage requestResult = await httpClient.Get(endPoint);
                string content = await requestResult.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IMessage>(content);
            }
            private async Task<IMessage> GetMessage(ITextChannel channel, string messageId)
                => await GetMessage(channel.Identifier, messageId);
            internal async Task<List<IMessage>> GetMessages(string channelId,
                                                            string messageId,
                                                            GetMessagesType type,
                                                            int limit = 50)
            {
                var queryParams = new NameValueCollection{
                                                            { type.ToString().ToLower(), messageId },
                                                            { "limit", limit.ToString() }
                                                         };
                string endPoint = $"/api/channels/{channelId}/messages"
                    .AddQueryParameters(queryParams);
                HttpResponseMessage requestResult = await httpClient.Get(endPoint);
                string content = await requestResult.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Message>>(content).Select(x => x as IMessage).ToList();
            }
            internal async Task<List<IMessage>> GetMessages(IChannel channel,
                                                            string messageId,
                                                            GetMessagesType type,
                                                            int limit = 50)
                => await GetMessages(channel.Identifier, messageId, type, limit);
            internal enum GetMessagesType : byte
            {
                Before,
                After,
                Around
            }
            private async Task<List<Ban>> GetGuildBannedUsers(string guildId)
            {
                string endPoint = $"/api/guilds/{guildId}/bans";
                HttpResponseMessage requestResult = await httpClient.Get(endPoint);
                string content = await requestResult.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Ban>>(content);
            }
            private async Task<List<Invite>> GetGuildInvites(string guildId)
            {
                string endPoint = $"/api/guilds/{guildId}/invites";
                HttpResponseMessage requestResult = await httpClient.Get(endPoint);
                string content = await requestResult.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Invite>>(content);
            }
            #endregion
        }
    }
    internal static class Extensions // TODO: переместить отсюда
    {
        internal static string AddQueryParameters(this string input, NameValueCollection parameters)
        {
            StringBuilder sb = new StringBuilder(input); // TODO : calc sb capacity
            string prefix = "?";
            for (var i = 0; i < parameters.Count; i++)
            {
                sb.Append(prefix + parameters.Keys[i] + "=" + parameters[i]);
                prefix = "&";
            }
            return sb.ToString();
        }
    }
}