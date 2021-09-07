using Newtonsoft.Json;

namespace DiscordDataObjects.Users.Activities
{
    internal class RichPresenceActivitySecrets
    {
        [JsonProperty(PropertyName = "join")]
        internal string Join { get; private set; }
        [JsonProperty(PropertyName = "spectate")]
        internal string Spectate { get; private set; }
        [JsonProperty(PropertyName = "match")]
        internal string Match { get; private set; }
    }
}
