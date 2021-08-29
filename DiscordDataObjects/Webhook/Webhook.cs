using Gateway.Entities.Channels;
using Gateway.Entities.Guilds;
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
		public string ApplicationIdentifier { get; private set; }
		public IGuild SourceGuild { get; private set; }
		public IChannel SourceChannel { get; private set; }
		public string Url { get; private set; }

		private User createdBy;
    }
}
