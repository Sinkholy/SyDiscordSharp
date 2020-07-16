using Gateway.Entities.Guilds;
using Gateway.Entities.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class GuildMember
    {
        internal IGuild Guild { get; private set; }
        internal IUser User => user as IUser;
        [JsonProperty]
        private User user;
        [JsonProperty(PropertyName = "guild_id")]
        private string guildIdentifier;
        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild = DiscordGatewayClient.TryToGetGuild(guildIdentifier);
        }
    }
}
