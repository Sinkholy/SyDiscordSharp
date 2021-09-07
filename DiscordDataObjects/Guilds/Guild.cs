using Gateway.Entities.Channels;
using Gateway.Entities.Channels.Guild;
using Gateway.Entities.Channels.Guild.IUpdatable;
using Gateway.Entities.Channels.Guild.Voice;
using Gateway.Entities.Emojis;
using Gateway.Entities.Invite;
using Gateway.Entities.Presences;
using Gateway.Entities.Users;
using Gateway.Entities.VoiceSession;
using Gateway.Payload.DataObjects.Dispatch.DispatchEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DiscordDataObjects.Guilds //TAI : ленивая загрузка всего и вся (ролей\пользователей etc.)
{
    internal class Guild : GuildPreview, IGuild, IUpdatableGuild
    {
        #region Private fields\properties
        [JsonProperty(PropertyName = "afk_timeout")]
        private int afkTimeout;
        [JsonProperty(PropertyName = "public_updates_channel_id")]
        private string publicUpdatesChannelIdentifier;
        [JsonProperty(PropertyName = "widget_channel_id")]
        private string widgetChannelIdentifier;
        [JsonProperty(PropertyName = "rules_channel_id")]
        private string rulesChannelIdentifier;
        [JsonProperty(PropertyName = "owner_id")]
        private string ownerIdentifier;
        [JsonProperty(PropertyName = "afk_channel_id")]
        private string afkChannelIdentifier;
        [JsonProperty(PropertyName = "system_channel_id")]
        private string systemChannelIdentifier;
        internal List<Ban> BannedUsersList = new List<Ban>(); //Смотри на инвайты
        internal List<IInvite> InvitesList = new List<IInvite>(); //TODO : ленивая загрузка
                                                                  //TODO : подгрузка данных при получении гильдии
        private Dictionary<string, Role> RolesById
        {
            get
            {
                if (_rolesById is null)
                {
                    _rolesById = new Dictionary<string, Role>();
                }
                return _rolesById;
            }
            set => _rolesById = value;
        }
        private Dictionary<string, Role> _rolesById;
        [JsonProperty(PropertyName = "roles")]
        private List<Role> _rolesReceived;
        internal Dictionary<string, GuildUser> UsersById // TODO : Потокобезопасность коллекций
        {
            get
            {
                if (_users is null)
                {
                    _users = new Dictionary<string, GuildUser>();
                }
                return _users;
            }
            set => _users = value;
        }
        private Dictionary<string, GuildUser> _users;
        [JsonProperty(PropertyName = "members")]
        private GuildUser[] usersReceived;
        internal Dictionary<string, IChannel> ChannelsById // TODO : Потокобезопасность коллекций
        {
            get
            {
                if (_channels is null)
                {
                    _channels = new Dictionary<string, IChannel>();
                }
                return _channels;
            }
            set => _channels = value;
        }
        private Dictionary<string, IChannel> _channels;
        [JsonProperty(PropertyName = "channels")]
        private List<IChannel> channelsReceived;
        #endregion
        #region IUpdatableGuild implementation
        Guild IUpdatableGuild.LoadInfoFromOldGuild(Guild oldGuildInfo)
        {
            Guild prevGuildInfo = this;
            JoinedAt = oldGuildInfo.JoinedAt;
            Large = oldGuildInfo.Large;
            Unavailable = oldGuildInfo.Unavailable;
            MemberCount = oldGuildInfo.MemberCount;
            _voiceSessionsById = oldGuildInfo.VoiceSessionsById;
            UsersById = oldGuildInfo.UsersById;
            //ChannelsList = oldGuildInfo.Channels as List<IChannel>; //TODO: писька
            PresencesByUserId = oldGuildInfo.PresencesByUserId;
            return prevGuildInfo;
        }
        void IUpdatableGuild.AddChannel(IChannel channel) => ChannelsById.Add(channel.Identifier, channel);
        void IUpdatableGuild.RemoveChannel(string channelId)
        {
            if (ChannelsById.ContainsKey(channelId))
            {
                ChannelsById.Remove(channelId);
            }
        }
        IChannel IUpdatableGuild.OverrideChannel(IChannel newChannelInfo)
        {
            IChannel oldChannelInfo = TryToGetChannel(newChannelInfo.Identifier);
            ChannelsById[newChannelInfo.Identifier] = newChannelInfo;
            return oldChannelInfo;
        }
        void IUpdatableGuild.AddRole(Role role) => RolesById.Add(role.Identifier, role);
        void IUpdatableGuild.RemoveRole(string roleId)
        {
            if (RolesById.ContainsKey(roleId))
            {
                RolesById.Remove(roleId);
            }
        }
        Role IUpdatableGuild.OverrideRole(Role newRoleInfo)
        {
            Role oldRoleInfo = TryToGetRole(newRoleInfo.Identifier);
            RolesById[newRoleInfo.Identifier] = newRoleInfo;
            return oldRoleInfo;
        }
        void IUpdatableGuild.AddUser(GuildUser user) => UsersById.Add(user.Identifier, user);
        void IUpdatableGuild.RemoveUser(string id)
        {
            if (UsersById.ContainsKey(id))
            {
                UsersById.Remove(id);
            }
        }
        void IUpdatableGuild.AddInvite(IInvite invite) => InvitesList.Add(invite);
        void IUpdatableGuild.RemoveInvite(string code)
        {
            IInvite inviteToDelete = TryToGetInvite(code);
            if (inviteToDelete != null)
            {
                InvitesList.Remove(inviteToDelete);
            }
        }
        void IUpdatableGuild.AddBan(Ban bannedUser) => BannedUsersList.Add(bannedUser);
        void IUpdatableGuild.RemoveBan(string userId)
        {
            Ban unbannedUser = TryToGetBannedUser(userId);
            if (unbannedUser != null)
            {
                BannedUsersList.Remove(unbannedUser);
            }
        }
        void IUpdatableGuild.AddVoiceSession(IVoiceSession session)
        {
            VoiceSessionsById.Add(session.SessionIdentifier, session);
        }
        IVoiceSession IUpdatableGuild.RemoveVoiceSession(string sessionId)
        {
            IVoiceSession deletedVoiceSession = VoiceSessionsById[sessionId];
            VoiceSessionsById.Remove(sessionId);
            return deletedVoiceSession;
        }
        GuildEmoji[] IUpdatableGuild.SetNewGuildEmojis(GuildEmoji[] emojis)
        {
            GuildEmoji[] prevEmojisInfo = _emojis;
            _emojis = emojis;
            return prevEmojisInfo;
        }
        GuildUser IUpdatableGuild.OverrideGuildUser(GuildUser newUser)
        {
            if (UsersById.TryGetValue(newUser.Identifier, out GuildUser user))
            {
                if (user is IUpdatableGuildUser updatableUser) // TODO: там ли я данные меняю?
                {
                    updatableUser.UpdateDeafenedState(user.Deafeaned);
                    updatableUser.UpdateMutedState(user.Muted);
                    updatableUser.UpdateRolesEnumerable(GetUserRoles(newUser.RolesIdentifiers));
                    updatableUser.UpdatePresencesEnumerable(GetUserPresence(newUser.Identifier));
                }
                UsersById[newUser.Identifier] = newUser;
                return user;
            }
            else
            {
                // TODO: логирование
                return null;
            }
        }
        IVoiceSession IUpdatableGuild.OverrideVoiceSession(IVoiceSession newStateInfo)
        {
            IVoiceSession prevSessionInfo = TryToGetVoiceSession(newStateInfo.SessionIdentifier);
            VoiceSessionsById[newStateInfo.SessionIdentifier] = newStateInfo;
            return prevSessionInfo;
        }
        IPresence IUpdatableGuild.OverridePresence(IPresence newPresenceInfo)
        {
            IPresence prevPresenceInfo = TryToGetPresence(newPresenceInfo.UserIdentifier);
            PresencesByUserId[newPresenceInfo.UserIdentifier] = newPresenceInfo;
            return prevPresenceInfo;
        }
        void IUpdatableGuild.AddPresence(IPresence newPresence)
        {
            PresencesByUserId.Add(newPresence.UserIdentifier, newPresence);
        }
        void IUpdatableGuild.RemovePresence(string userId)
        {
            if (PresencesByUserId.ContainsKey(userId))
            {
                PresencesByUserId.Remove(userId);
            }
        }
        #endregion
        #region IGuild implementation
        public IChannel PublicUpdatesChannel //Смотри на канал виджетов
            => ChannelsById.Values.Where(x => x.Identifier == publicUpdatesChannelIdentifier).SingleOrDefault();
        public IChannel WidgetChannel //TODO : узнать что из себя представляет Widget-канал и изменить тип
            => ChannelsById.Values.Where(x => x.Identifier == widgetChannelIdentifier).SingleOrDefault();
        public ITextChannel RulesChannel
            => ChannelsById.Values.Where(x => x.Identifier == rulesChannelIdentifier)
                       .Select(x => x as ITextChannel)
                       .SingleOrDefault();
        public ITextChannel SystemChannel
            => ChannelsById.Values.Where(x => x.Identifier == systemChannelIdentifier)
                       .Select(x => x as ITextChannel)
                       .SingleOrDefault();
        public IVoiceChannel AfkChannel //Например в этом поле можно впихнуть ленивую загрузку
            => ChannelsById.Values.Where(x => x.Identifier == afkChannelIdentifier)
                       .Select(x => x as IVoiceChannel)
                       .SingleOrDefault();
        public IUser Owner
            => UsersById[ownerIdentifier];
        public IReadOnlyCollection<IChannel> Channels => ChannelsById.Values.ToList();
        public IReadOnlyCollection<IGuildCategory> ChannelCategories
            => ChannelsById.Values.Where(x => x is IGuildCategory)
                       .Select(x => x as IGuildCategory)
                       .ToList() as IReadOnlyCollection<IGuildCategory>;
        public IReadOnlyCollection<IVoiceChannel> VoiceChannels
            => ChannelsById.Values.Where(x => x is IVoiceChannel)
                       .Select(x => x as IVoiceChannel)
                       .ToList() as IReadOnlyCollection<IVoiceChannel>;
        public IReadOnlyCollection<ITextChannel> TextChannels
            => ChannelsById.Values.Where(x => x is ITextChannel)
                       .Select(x => x as ITextChannel)
                       .ToList() as IReadOnlyCollection<ITextChannel>;
        public IReadOnlyCollection<Role> Roles => RolesById.Values;
        public IReadOnlyCollection<IInvite> Invites => InvitesList as IReadOnlyCollection<IInvite>;
        public IReadOnlyCollection<IUser> Users => UsersById.Values;
        public IReadOnlyCollection<IUser> BannedUsers => BannedUsersList as IReadOnlyCollection<IUser>;
        public IReadOnlyCollection<GuildEmoji> Emojis => _emojis;
        public IReadOnlyCollection<IVoiceSession> ActiveVoiceSessions => VoiceSessionsById.Values;
        public IReadOnlyCollection<IPresence> Presences => PresencesByUserId.Values;
        [JsonProperty(PropertyName = "owner")]
        public bool IsOwner { get; private set; }
        [JsonProperty(PropertyName = "permissions")]
        public int? Permissions { get; private set; }
        [JsonProperty(PropertyName = "mfa_level")]
        public MFA MultifactorAuth { get; private set; }
        [JsonProperty(PropertyName = "application_id")]
        public string ApplicationIdentifier { get; private set; }
        [JsonProperty(PropertyName = "widget_enabled")]
        public bool WigdetEnabled { get; private set; }
        [JsonProperty(PropertyName = "system_channel_flags")]
        public SystemChannelFlags SysChnlFlags { get; private set; }
        [JsonProperty(PropertyName = "verification_level")]
        public VerificationLevel VerificationLevel { get; private set; }
        [JsonProperty(PropertyName = "default_message_notification")]
        public DefaultMessageNotificationLevel DefaultMessageNotificationLevel { get; private set; }
        [JsonProperty(PropertyName = "explicit_content_filter")]
        public ExplicitContentFilterLevel ExplicitContentFilterLevel { get; private set; }
        [JsonProperty(PropertyName = "joined_at")]
        public DateTime JoinedAt { get; private set; }
        [JsonProperty(PropertyName = "large")]
        public bool Large { get; private set; }
        [JsonProperty(PropertyName = "member_count")]
        public int MemberCount { get; private set; }

        internal Dictionary<string, IVoiceSession> VoiceSessionsById
        {
            get
            {
                if (_voiceSessionsById is null)
                {
                    _voiceSessionsById = new Dictionary<string, IVoiceSession>();
                }
                return _voiceSessionsById;
            }
            private set => _voiceSessionsById = value;
        }
        private Dictionary<string, IVoiceSession> _voiceSessionsById;
        [JsonProperty(PropertyName = "voice_states")]
        private IVoiceSession[] voiceSessionsReceived;

        internal Dictionary<string, IPresence> PresencesByUserId
        {
            get
            {
                if (_presencesByUserId is null)
                {
                    _presencesByUserId = new Dictionary<string, IPresence>();
                }
                return _presencesByUserId;
            }
            set => _presencesByUserId = value;
        }
        private Dictionary<string, IPresence> _presencesByUserId;
        [JsonProperty(PropertyName = "presences")]
        internal IPresence[] presencesReceived;
        public TimeSpan? AfkTimeout => TimeSpan.FromSeconds(afkTimeout);
        [JsonProperty(PropertyName = "max_presences")]
        public int? MaxPresences { get; private set; } = 25000;
        [JsonProperty(PropertyName = "region")]
        public string Region { get; private set; }
        [JsonProperty(PropertyName = "max_members")]
        public int MaxMembers { get; private set; }
        [JsonProperty(PropertyName = "vanity_url_code")]
        public string VanityUrlCode { get; private set; }
        [JsonProperty(PropertyName = "banner")]
        public string Banner { get; private set; }
        [JsonProperty(PropertyName = "premium_tier")]
        public PremiumTier PremTier { get; private set; }
        [JsonProperty(PropertyName = "premium_subscriptions_count")]
        public int PremiumSubscriptionsCount { get; private set; }
        [JsonProperty(PropertyName = "preferred_locale")]
        public string PreferredLocale { get; private set; } //TAI : может локали запихнуть в перечисление?
        [JsonProperty(PropertyName = "max_video_channel_users")]
        public int MaxVideoChannelUsers { get; private set; }
        public IChannel TryToGetChannel(string id) //TAI : каналы\юзеров\роли запихать в словари для быстрого доступа?
        {                                          //может быть актуально при частом доступе(а он будет, по идее);
            if (ChannelsById.TryGetValue(id, out IChannel channel))
            {
                return channel;
            }
            return null;
        }
        public IUser TryToGetUser(string id)
        {
            if (UsersById.TryGetValue(id, out GuildUser user))
            {
                return user;
            }
            return null;
        }
        public Role TryToGetRole(string id)
        {
            if (RolesById.TryGetValue(id, out Role role))
            {
                return role;
            }
            return null;
        }
        public IInvite TryToGetInvite(string code)
        {
            return InvitesList.Where(x => x.Code == code).SingleOrDefault();
        }
        public Ban TryToGetBannedUser(string userId)
        {
            return BannedUsersList.Where(x => x.User.Identifier == userId).SingleOrDefault();
        }
        public IVoiceSession TryToGetVoiceSession(string sessionId)
        {
            if (VoiceSessionsById.TryGetValue(sessionId, out IVoiceSession session))
            {
                return session;
            }
            return null;
        }
        public GuildEmoji TryToGetEmoji(string id)
        {
            return _emojis.Where(x => x.Identifier == id).SingleOrDefault();
        }
        public IPresence TryToGetPresence(string userId)
        {
            if (PresencesByUserId.TryGetValue(userId, out IPresence presence))
            {
                return presence;
            }
            return null;
        }
        #endregion
        #region Deserialization methods
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (usersReceived != null)
            {
                foreach (GuildUser user in usersReceived)
                {
                    if (user is IUpdatableGuildUser updatableUser)
                    {
                        updatableUser.UpdateRolesEnumerable(GetUserRoles(user.RolesIdentifiers));
                        updatableUser.UpdatePresencesEnumerable(GetUserPresence(user.Identifier));
                        updatableUser.UpdateGuildIdentifier(this.Identifier);
                    }
                    if (UsersById.ContainsKey(user.Identifier))
                    {
                        // логировать получение дубликата
                        Console.WriteLine($"ПОЙМАЛ ДУБЛИКАТ В ЮЗЕРАХ АХТУНГ: {user.FullName}");
                    }
                    else
                    {
                        UsersById.Add(user.Identifier, user);
                    }
                }
                usersReceived = null;
            }
            if (channelsReceived != null)
            {
                foreach (IGuildChannel channel in channelsReceived)
                {
                    if(channel is IUpdatableGuildChannel updatableChannel)
                    {
                        updatableChannel.SetNewGuildId(this.Identifier);
                    }
                    if (channel.Type == ChannelType.GuildVoice)
                    {
                        (channel as GuildVoiceChannel).activeVoiceSessionsEnumerable = GetActiveSessionsForChannel(channel.Identifier);
                    }
                    ChannelsById.Add(channel.Identifier, channel);
                }
                channelsReceived = null;
            }
            if (voiceSessionsReceived != null)
            {
                foreach (IVoiceSession session in voiceSessionsReceived)
                {
                    if (session is IUpdatableVoiceSession updatableSession)
                    {
                        updatableSession.SetGuildId(this.Identifier);
                    }
                    if (this is IUpdatableGuild updatableGuild)
                    {
                        updatableGuild.AddVoiceSession(session);
                    }
                }
                voiceSessionsReceived = null;
            }
            if (presencesReceived != null)
            {
                foreach (IPresence presence in presencesReceived)
                {
                    PresencesByUserId.Add(presence.UserIdentifier, presence);
                }
                presencesReceived = null;
            }
            if (_rolesReceived != null)
            {
                foreach (Role role in _rolesReceived)
                {
                    RolesById.Add(role.Identifier, role);
                }
                _rolesReceived = null;
            }
        }

        private IEnumerable<Role> GetUserRoles(string[] rolesId)
        {
            foreach (string roleId in rolesId)
            {
                yield return TryToGetRole(roleId);
            }
        }
        private IEnumerable<IVoiceSession> GetActiveSessionsForChannel(string channelId)
        {
            foreach (IVoiceSession session in ActiveVoiceSessions.Where(x => x.ChannelIdentifier == channelId))
            {
                yield return session;
            }
        }
        private IEnumerable<IPresence> GetUserPresence(string userId)
        {
            yield return TryToGetPresence(userId);
        }
        #endregion
        #region Ctor's
        internal Guild() { }
        #endregion
    }
}