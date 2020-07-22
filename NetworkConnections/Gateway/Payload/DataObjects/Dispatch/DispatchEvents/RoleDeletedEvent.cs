using Newtonsoft.Json;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class RoleDeletedEvent
    {
        [JsonProperty(PropertyName = "role")]
        internal string RoleIdentifier{ get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
    }
}