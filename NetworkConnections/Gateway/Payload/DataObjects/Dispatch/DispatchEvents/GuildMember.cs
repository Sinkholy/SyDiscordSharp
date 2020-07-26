using Gateway.Entities.Users;
using Newtonsoft.Json;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class GuildMember
    {
        [JsonProperty(PropertyName = "user")]
        internal User User { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
    }
}
