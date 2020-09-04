using Gateway.Entities.Emojis;
using Newtonsoft.Json;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class MessageReactionEvent
    {
        [JsonProperty(PropertyName = "message_id")]
        internal string MessageIdentifier { get; private set; }
        [JsonProperty(PropertyName = "emoji")]
        internal IEmoji Emoji { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "channel_id")]
        internal string ChannelIdentifier { get; private set; }
        [JsonProperty(PropertyName = "user_id")]
        internal string UserIdentifier { get; private set; }
    }
}
