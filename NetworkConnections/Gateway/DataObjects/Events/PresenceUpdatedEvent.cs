using Gateway.DataObjects.Guilds;
using Gateway.DataObjects.Roles;
using Gateway.DataObjects.Users;
using Gateway.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Events
{
    internal class PresenceUpdatedEvent
    {
        internal IUser User => user as IUser;
        internal IGuild Guild { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "roles")]
        internal Role[] RolesIdentifiers;
        [JsonProperty(PropertyName = "activity")]
        internal Activity Game { get; private set; }
        [JsonProperty(PropertyName = "activities")]
        internal Activity[] Activities { get; private set; }
        [JsonProperty(PropertyName = "premium_since")]
        internal DateTime PremiumSince { get; private set; }
        [JsonProperty(PropertyName = "status")]
        internal UserStatus Status { get; private set; }
        [JsonProperty(PropertyName = "client_status")]
        internal ClientStatus ClientStatuses { get; private set; }
        internal string Nickname { get; private set; }

        [JsonProperty(PropertyName = "user")]
        private User user;

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild = DiscordGatewayClient.TryToGetGuild(GuildIdentifier);
        }
        /// <summary>
        /// Represents client online states for all platforms
        /// </summary>
        /// <remarks>
        /// Fields are partial UserStatus enum, only can have: Online, Idle, Dnd 
        /// otherwise field isnt present
        /// </remarks>
        internal class ClientStatus
        {
            [JsonProperty(PropertyName = "desktop")]
            internal UserStatus? Desktop { get; private set; }
            [JsonProperty(PropertyName = "mobile")]
            internal UserStatus? Mobile { get; private set; }
            [JsonProperty(PropertyName = "web")]
            internal UserStatus? Web { get; private set; }
        }
    }
}
