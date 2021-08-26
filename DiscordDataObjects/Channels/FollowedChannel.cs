using Newtonsoft.Json;

namespace Gateway.Entities
{
    public class FollowedChannel
    {
        [JsonProperty(PropertyName = "channel_id")]
        public string ChannelIdentifier { get; private set; }
        [JsonProperty(PropertyName = "webhook_id")]
        public string WebhookIdentifier { get; private set; }
    }
}
