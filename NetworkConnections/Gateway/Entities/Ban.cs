using Gateway.Entities.Users;
using Newtonsoft.Json;

namespace Gateway.Entities
{
    internal class Ban
    {
        [JsonProperty(PropertyName = "user")]
        internal User User { get; private set; }

        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
    }
}
