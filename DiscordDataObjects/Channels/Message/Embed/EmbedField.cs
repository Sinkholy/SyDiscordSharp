using Newtonsoft.Json;

namespace Gateway.Entities.Embed
{
    public class EmbedField
    {
        public static string WhiteSpace => "\u200b";
        /// <summary>
        /// Limit: 256 characters
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        /// <summary>
        /// Limit: 1024 characters
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; private set; }
        [JsonProperty(PropertyName = "inline")]
        public bool Inline { get; private set; }

        public EmbedField(string name,
                          string value,
                          bool inline)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }
    }
}
