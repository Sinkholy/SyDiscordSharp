using Gateway.Entities;
using Gateway.Entities.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class RoleEvent
    {
        internal IGuild Guild { get; private set; }
        [JsonProperty(PropertyName = "role")]
        internal Role Role { get; private set; }
        [JsonProperty(PropertyName = "role_id")]
        internal string RoleIdentifier { get; private set; }
        
        [JsonProperty(PropertyName = "guild_id")]
        private string guildIdentifier;

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            if (Role != null)
                RoleIdentifier = Role.Identifier;
            Guild = DiscordGatewayClient.TryToGetGuild(guildIdentifier);
        }
    }
}
