using Gateway.DataObjects.Voice;
using Gateway.Entities.Channels;
using Gateway.Entities.Channels.Text;
using Gateway.Entities.Channels.Voice;
using Gateway.Entities.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
namespace Gateway.Entities.Guilds //TAI : ленивая загрузка всего и вся (ролей\пользователей etc.)
{
    internal class Guild : GuildPreview
    {
        #region RO collections
        public IReadOnlyCollection<IChannel> Channels => channels;
        public IReadOnlyCollection<ChannelCategory> ChannelCategories
            => channels.Where(x => x is ChannelCategory)
                       .Select(x => x as ChannelCategory)
                       .ToList() as IReadOnlyCollection<ChannelCategory>;
        public IReadOnlyCollection<IVoiceChannel> VoiceChannels
            => channels.Where(x => x is IVoiceChannel)
                       .Select(x => x as IVoiceChannel)
                       .ToList() as IReadOnlyCollection<IVoiceChannel>;
        public IReadOnlyCollection<ITextChannel> TextChannels
            => channels.Where(x => x is ITextChannel)
                       .Select(x => x as ITextChannel)
                       .ToList() as IReadOnlyCollection<ITextChannel>;
        #endregion
        #region Public fields\properties
        public IChannel PublicUpdatesChannel  //Смотри на канал виджетов
            => channels.Where(x => x.Identifier == publicUpdatesChannelIdentifier).SingleOrDefault();
        public IChannel WidgetChannel //TODO : узнать что из себя представляет Widget-канал и изменить тип
            => channels.Where(x => x.Identifier == widgetChannelIdentifier).SingleOrDefault();
        public ITextChannel RulesChannel
            => channels.Where(x => x.Identifier == rulesChannelIdentifier)
                       .Select(x => x as ITextChannel)
                       .SingleOrDefault();
        public ITextChannel SystemChannel
            => channels.Where(x => x.Identifier == systemChannelIdentifier)
                       .Select(x => x as ITextChannel)
                       .SingleOrDefault();
        public IVoiceChannel AfkChannel //Например в этом поле можно впихнуть ленивую загрузку
            => channels.Where(x => x.Identifier == afkChannelIdentifier)
                       .Select(x => x as IVoiceChannel)
                       .SingleOrDefault();
        public IUser Owner
            => Users.Where(x => (x as IUser).Identifier == ownerIdentifier)
                                            .SingleOrDefault() as IUser;
        #endregion
        #region internal fields\properties
        [JsonProperty(PropertyName = "owner")]
        internal bool IsOwner;
        [JsonProperty(PropertyName = "permissions")]
        internal int Permissions;
        [JsonProperty(PropertyName = "mfa_level")]
        internal MFA MultifactorAuth;
        [JsonProperty(PropertyName = "application_id")]
        internal string ApplicationIdentifier;
        [JsonProperty(PropertyName = "widget_enabled")]
        internal bool WigdetEnabled;
        [JsonProperty(PropertyName = "system_channel_flags")]
        internal SystemChannelFlags SysChnlFlags;
        [JsonProperty(PropertyName = "verification_level")]
        internal VerificationLevel VerificationLevel;
        [JsonProperty(PropertyName = "default_message_notification")]
        internal DefaultMessageNotificationLevel DefaultMessageNotificationLevel;
        [JsonProperty(PropertyName = "explicit_content_filter")]
        internal ExplicitContentFilterLevel ExplicitContentFilterLevel;
        [JsonProperty(PropertyName = "joined_at")]
        internal DateTime JoinedAt;
        [JsonProperty(PropertyName = "large")]
        internal bool Large;
        [JsonProperty(PropertyName = "member_count")]
        internal int MemberCount;
        [JsonProperty(PropertyName = "voice_states")]
        internal VoiceState[] VoiceStates;
        //[JsonProperty(PropertyName = "presences")]
        //internal Presence[] Presences;
        internal TimeSpan? AfkTimeout;
        [JsonProperty(PropertyName = "max_presences")]
        internal int? MaxPresences = 25000;
        [JsonProperty(PropertyName = "region")]
        internal string Region;
        [JsonProperty(PropertyName = "max_members")]
        internal int MaxMembers;
        [JsonProperty(PropertyName = "vanity_url_code")]
        internal string VanityUrlCode;
        [JsonProperty(PropertyName = "banner")]
        internal string Banner;
        [JsonProperty(PropertyName = "premium_tier")]
        internal PremiumTier PremTier;
        [JsonProperty(PropertyName = "premium_subscriptions_count")]
        internal int PremiumSubscriptionsCount;
        [JsonProperty(PropertyName = "preferred_locale")]
        internal string PreferredLocale; //TAI : может локали запихнуть в перечисление?
        [JsonProperty(PropertyName = "max_video_channel_users")]
        internal int MaxVideoChannelUsers;
        #endregion
        #region private fields\properties
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
        [JsonProperty(PropertyName = "roles")]
        internal List<Role> Roles
        {
            get => _roles ?? new List<Role>(capacity: 0);
            set => _roles = value;
        }
        private List<Role> _roles;
        [JsonProperty(PropertyName = "members")]
        internal List<GuildUser> Users
        {
            get => _users ?? new List<GuildUser>(capacity: 0);
            set => _users = value;
        }
        private List<GuildUser> _users;
        private List<IChannel> channels 
        {
            get => _channels ?? new List<IChannel>(capacity: 0);
            set => _channels = value;
        }
        [JsonProperty(PropertyName = "channels")]
        private List<IChannel> _channels;
        #endregion
        public void UpdateGuild(Guild newGuildInfo)
        {
            
        }
        internal void AddChannel(IChannel channel)
        {
            channels.Add(channel);
        }
        internal void RemoveChannel(string channelId)
        {
            IChannel channelToRemove = channels.Where(x => x.Identifier == channelId).SingleOrDefault();
            if (channelToRemove != null)
            {
                channels.Remove(channelToRemove);
            }
        }
        internal IChannel TryToGetChannel(string id) //TAI : каналы\юзеров\роли запихать в словари для быстрого доступа?
        {                                            //может быть актуально при частом доступе(а он будет, по идее);
            return channels.Where(x => x.Identifier == id).SingleOrDefault();
        }
        internal IUser TryToGetUser(string id)
        {
            return Users.Where(x => (x as IUser).Identifier == id).SingleOrDefault() as IUser;
        }
        internal Role TryToGetRole(string id)
        {
            return Roles.Where(x => x.Identifier == id).SingleOrDefault();
        }
        #region Deserialization methods
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            AfkTimeout = TimeSpan.FromSeconds(afkTimeout);
            UpdateChennelsGuildId();
            UpdateUsers();
        }
        private void UpdateChennelsGuildId()
        {
            foreach(IGuildChannel channel in channels)
                channel.UpdateChannelGuildId(this as IGuild);
        }
        private void UpdateUsers()
        {
            foreach (GuildUser user in Users)
            {
                foreach (string roleId in user.RolesIdentifiers)
                {
                    Role role = Roles.Where(x => x.Identifier == roleId).SingleOrDefault();
                    user.roles.Add(role);
                }
                if (user.Guild == null)
                    user.Guild = this;
            }
        }
        #endregion
    }
    internal enum PremiumTier : byte
    {
        None,
        FirstTier,
        SecondTier,
        ThirdTier
    }
    internal enum MFA : byte { None, Elevated }
    internal enum DefaultMessageNotificationLevel : byte { AllMessages, OnlyMentions }
    internal enum ExplicitContentFilterLevel : byte
    {
        Disabled,
        MembersWithoutRoles,
        AllMembers
    }
    internal enum VerificationLevel : byte
    {
        None,
        Low,
        Medium,
        High,
        VeryHigh
    }
    [Flags]
    internal enum SystemChannelFlags
    {
        SuppressJoinNotification     = 1 << 0,
        SuppressPremiumSubscriptions = 1 << 1
    }
}