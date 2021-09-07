using Newtonsoft.Json;

namespace DiscordDataObjects.Channels.Message.Embed
{
    public class EmbedFooter
    {
        /// <summary>
        /// Limit: 2048 characters
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; private set; }
        [JsonProperty(PropertyName = "icon_url")]
        public string IconUri { get; private set; }
        [JsonProperty(PropertyName = "proxy_icon_url")]
        public string ProxyIconUri { get; private set; }

        public EmbedFooter(string text,
                           string iconUri,
                           string proxyIconUri)
        {
            Text = text;
            IconUri = iconUri;
            ProxyIconUri = proxyIconUri;
        }
    }
}
