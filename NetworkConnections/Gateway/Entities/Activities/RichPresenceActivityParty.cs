using Newtonsoft.Json;

namespace Gateway.Entities.Activities
{
    internal class RichPresenceActivityParty
    {
        [JsonProperty(PropertyName = "id")]
        internal string Identifier { get; private set; }
        internal int? CurrentSize => size[0];
        internal int? MaxSize => size[1];

        [JsonProperty(PropertyName = "size")]
        private int?[] size;
    }
}
