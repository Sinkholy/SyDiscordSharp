using Newtonsoft.Json;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    class MessageDeletedBulk
    {
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "channel_id")]
        internal string ChannelIdentifier { get; private set; }
        [JsonProperty(PropertyName = "ids")]
        internal string[] Messages { get; private set; }
    }
}
