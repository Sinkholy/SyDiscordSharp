using DiscordDataObjects.Users;

using Newtonsoft.Json;

namespace DiscordDataObjects.Guilds
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
