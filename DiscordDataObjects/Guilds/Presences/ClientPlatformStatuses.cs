using Newtonsoft.Json;

namespace DiscordDataObjects.Guilds.Presences
{
    /// <summary>
    /// Represents client online states for all platforms
    /// </summary>
    /// <remarks>
    /// Fields are partial UserStatus enum, only can have: Online, Idle, Dnd 
    /// otherwise field isnt present
    /// </remarks>
    public class ClientPlatformStatuses
    {
        [JsonProperty(PropertyName = "desktop")]
        public UserStatus? Desktop { get; private set; }
        [JsonProperty(PropertyName = "mobile")]
        public UserStatus? Mobile { get; private set; }
        [JsonProperty(PropertyName = "web")]
        public UserStatus? Web { get; private set; }
    }
}
