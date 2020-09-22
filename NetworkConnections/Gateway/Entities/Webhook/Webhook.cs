using Gateway.Entities.Users;
using Newtonsoft.Json;

namespace Gateway.Entities.Webhook
{
    public class Webhook : IWebhook
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "type")]
        public WebhookType Type { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "channel_id")]
        public string ChannelIdentifier { get; private set; }
        public IUser CreatedBy => createdBy;
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "avatar")]
        public string Avatar { get; private set; }
        [JsonProperty(PropertyName = "token")]
        public string Token { get; private set; }

        private User createdBy;
    }
}
