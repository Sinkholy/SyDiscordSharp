using Newtonsoft.Json;

namespace DiscordDataObjects.Channels.Message.Embed
{
    public class EmbedImage
    {
        [JsonProperty(PropertyName = "url")]
        public string Uri { get; private set; }
        [JsonProperty(PropertyName = "proxy_url")]
        public string ProxyUri { get; private set; }
        [JsonProperty(PropertyName = "height")]
        public int Height { get; private set; }
        [JsonProperty(PropertyName = "width")]
        public int Width { get; private set; }

        public EmbedImage(string uri,
                          string proxyUri,
                          int height,
                          int width)
        {
            Uri = uri;
            ProxyUri = uri;
            Height = height;
            Width = width;
        }
    }
}
