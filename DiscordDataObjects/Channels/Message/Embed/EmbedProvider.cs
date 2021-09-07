using Newtonsoft.Json;

namespace DiscordDataObjects.Channels.Message.Embed
{
    public class EmbedProvider
    {
        [JsonProperty(PropertyName = "name")]   
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; private set; }
    }
}
