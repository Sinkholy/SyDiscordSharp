using Newtonsoft.Json;

namespace Gateway.Entities.Embed
{
    public class EmbedProvider
    {
        [JsonProperty(PropertyName = "name")]   
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; private set; }
    }
}
