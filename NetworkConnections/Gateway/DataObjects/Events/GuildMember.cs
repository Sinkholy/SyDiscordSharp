using Gateway.DataObjects.Guilds;
using Gateway.DataObjects.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Events
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
