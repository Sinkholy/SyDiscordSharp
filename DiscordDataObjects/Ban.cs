using Gateway.Entities.Users;
using Newtonsoft.Json;

namespace Gateway.Entities
{
    public class Ban
    {
        [JsonProperty(PropertyName = "user")]
        internal User User { get; private set; }

        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }

        [JsonProperty(PropertyName = "reason")]
        internal string Reason { get; private set; }
    }
}
