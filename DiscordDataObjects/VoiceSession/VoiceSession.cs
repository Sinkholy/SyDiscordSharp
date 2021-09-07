using System;

using Gateway.Entities.Users;
using Newtonsoft.Json;

namespace DiscordDataObjects.VoiceSession
{
    internal class VoiceSession : IVoiceSession, IUpdatableVoiceSession
    {
        #region IVoiceSession implementation
        [JsonProperty(PropertyName = "user_id")]
        public string UserIdentifier { get; private set; }
        public IGuildUser User => guildUser;
        [JsonProperty(PropertyName = "channel_id")]
        public string ChannelIdentifier { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "session_id")]
        public string SessionIdentifier { get; private set; }
        [JsonProperty(PropertyName = "deaf")]
        public bool Deafened { get; private set; }
        [JsonProperty(PropertyName = "mute")]
        public bool Muted { get; private set; }
        [JsonProperty(PropertyName = "self_deaf")]
        public bool SelfDeafened { get; private set; }
        [JsonProperty(PropertyName = "self_mute")]
        public bool SelfMuted { get; private set; }
        [JsonProperty(PropertyName = "self_stream")]
        public bool SelfStream { get; private set; }
        [JsonProperty(PropertyName = "self_video")]
        public bool SelfVideo { get; private set; }
        [JsonProperty(PropertyName = "suppress")]
        public bool Suppressed { get; private set; }
		public DateTime RequestToSpeakTimestamp { get; }
		#endregion


		[JsonProperty(PropertyName = "member")]
        private GuildUser guildUser;

        void IUpdatableVoiceSession.SetGuildId(string guildId)
        {
            GuildIdentifier = guildId;
        }
    }
}
