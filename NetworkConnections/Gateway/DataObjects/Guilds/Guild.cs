using Gateway.DataObjects.Channels;
using Gateway.DataObjects.Channels.Text;
using Gateway.DataObjects.Channels.Voice;
using Gateway.DataObjects.Users;
using Gateway.DataObjects.Voice;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Gateway.DataObjects.Guilds //TAI : ленивая загрузка всего и вся (ролей\пользователей etc.)
{
    [JsonObject(MemberSerialization.OptOut)]
    internal class Guild : GuildPreview
    {
        #region internal fields\properties
        internal IChannel PublicUpdatesChannel  //Смотри на канал виджетов
            => channels.Where(x => x.Identifier == publicUpdatesChannelIdentifier).SingleOrDefault();
        internal IChannel WidgetChannel //TODO : узнать что из себя представляет Widget-канал и изменить тип
            => channels.Where(x => x.Identifier == widgetChannelIdentifier).SingleOrDefault();
        internal ITextChannel RulesChannel 
            => channels.Where(x => x.Identifier == rulesChannelIdentifier)
                       .Select(x => x as ITextChannel)
                       .SingleOrDefault(); 
        internal ITextChannel SystemChannel 
            => channels.Where(x => x.Identifier == systemChannelIdentifier)
                       .Select(x => x as ITextChannel)
                       .SingleOrDefault();
        internal IVoiceChannel AfkChannel //Например в этом поле можно впихнуть ленивую загрузку
            => channels.Where(x => x.Identifier == afkChannelIdentifier)
                       .Select(x => x as IVoiceChannel)
                       .SingleOrDefault();
        internal GuildUser Owner 
            => Users.Where(x => x.Identifier == ownerIdentifier)
                    .SingleOrDefault();

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
        [JsonProperty(PropertyName = "roles")]
        internal Role[] Roles;
        [JsonProperty(PropertyName = "joined_at")]
        internal DateTime JoinedAt;
        [JsonProperty(PropertyName = "large")]
        internal bool Large;
        [JsonProperty(PropertyName = "unavailable")]
        internal bool Unavailable;
        [JsonProperty(PropertyName = "member_count")]
        internal int MemberCount;
        [JsonProperty(PropertyName = "voice_states")]
        internal VoiceState[] VoiceStates;
        [JsonProperty(PropertyName = "members")]
        internal GuildUser[] Users;
        [JsonProperty(PropertyName = "presences")]
        internal Presence[] Presences;
        [JsonProperty(PropertyName = "afk_timeout")]
        [JsonConverter(typeof(GuildAfkTimeoutConverter))]
        internal TimeSpan AfkTimeout;
        [JsonProperty(PropertyName = "max_presences")]
        internal int MaxPresences = 25000; //TODO : при получении null-значения(json), необходимо 
                                           //устанавливать стандартное значение, на данный момент == 25000
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
        [JsonProperty(PropertyName = "channels")]
        private IChannel[] channels;
        #endregion
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


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            UpdateChennelsGuildId();
            UpdateUsersRoles();
        }
        private void UpdateChennelsGuildId()
        {
            foreach(IChannel channel in channels)
                (channel as IGuildChannel).UpdateChannelGuildId(this as IGuild);
        }
        private void UpdateUsersRoles()
        {
            foreach (GuildUser user in Users)
            {
                foreach (string roleId in user.rolesId)
                {
                    Role role = this.Roles.Where(x => x.Identifier == roleId).SingleOrDefault();
                    user.roles.Add(role);
                }
            }
        }

        private class GuildAfkTimeoutConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(TimeSpan);
            public override bool CanRead => true;
            public override bool CanWrite => false;
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JToken input = JToken.Load(reader);
                double seconds = double.Parse(input.ToString());
                return TimeSpan.FromSeconds(seconds);
            }
            /// <summary>
            /// Not implemented
            /// </summary>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                => throw new NotImplementedException();
        }
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