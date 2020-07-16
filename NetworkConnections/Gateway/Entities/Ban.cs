using Gateway.Entities.Guilds;
using Gateway.Entities.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities
{
    internal class Ban
    {
        internal IGuild Guild { get; private set; }
        [JsonProperty(PropertyName = "user")]
        internal User User { get; private set; }

        [JsonProperty(PropertyName = "guild_id")]
        private string guildIdentifier;       

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild = DiscordGatewayClient.TryToGetGuild(guildIdentifier);
        }
    }
}
