using Gateway.Entities.Emojis;
using Gateway.Entities.Guilds;
using Newtonsoft.Json;

namespace Gateway.Entities.Message
{
    public class MessageBase
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "channel_id")]
        public string ChannelIdentifier { get; private set; }
    }
}
