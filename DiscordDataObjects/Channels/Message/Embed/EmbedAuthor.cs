using Newtonsoft.Json;

namespace DiscordDataObjects.Channels.Message.Embed
{
    public class EmbedAuthor
    {
        /// <summary>
        /// Limit: 256 characters
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "url")]
        public string Uri { get; private set; }
        [JsonProperty(PropertyName = "icon_url")]
        public string IconUri { get; private set; }
        [JsonProperty(PropertyName = "proxy_icon_url")]
        public string ProxyIconUri { get; private set; }

        public EmbedAuthor(string name,
                           string uri,
                           string iconUri,
                           string proxyIconUri)
        {
            Name = name;
            Uri = uri;
            IconUri = iconUri;
            ProxyIconUri = proxyIconUri;
        }
    }
}
