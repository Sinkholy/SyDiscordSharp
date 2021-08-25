using Newtonsoft.Json;

namespace Gateway.Entities.Embed
{
    public class EmbedVideo
    {
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; private set; }
        [JsonProperty(PropertyName = "height")]
        public int Height { get; private set; }
        [JsonProperty(PropertyName = "width")]
        public int Width { get; private set; }

        public EmbedVideo(string uri,
                          int height,
                          int width)
        {
            Uri = uri;
            Height = height;
            Width = width;
        }
    }
}
