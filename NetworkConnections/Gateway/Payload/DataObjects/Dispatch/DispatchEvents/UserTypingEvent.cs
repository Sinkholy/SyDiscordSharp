using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class UserTypingEvent
    {
        internal DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
        internal bool InGuild { get; private set; } = false;

        [JsonProperty(PropertyName = "timestamp")]
        private int timestamp;
        [JsonProperty(PropertyName = "guild_id")]
        private string guildIdentifier;
        [JsonProperty(PropertyName = "channel_id")]
        private string channelIdentifier;
        [JsonProperty(PropertyName = "user_id")]
        private string userIdentifier;
        [JsonProperty(PropertyName = "member")]
        private IGuildUser user;
    }
}
