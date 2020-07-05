using Gateway.DataObjects.Guilds;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Users
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GuildUser : IUser
    {
        public IReadOnlyCollection<Role> Roles => roles as IReadOnlyCollection<Role>;

        internal List<Role> roles;
        [JsonProperty(PropertyName = "roles")]
        internal string[] rolesId;
        [JsonProperty(PropertyName = "user")]
        internal User User;
        [JsonProperty(PropertyName = "nick")]
        internal string Nickname;
        [JsonProperty(PropertyName = "joined_at")]
        internal DateTime JoinedAt;
        [JsonProperty(PropertyName = "premium_since")]
        internal DateTime? PremiumSince;
        [JsonProperty(PropertyName = "deaf")]
        internal bool Deafeaned;
        [JsonProperty(PropertyName = "mute")]
        internal bool Muted;

        [OnDeserialized]
        private void FinishInicialization(StreamingContext context)
        {
            roles = new List<Role>(capacity: rolesId.Length);
        }
        #region IUser implementation
        public string Identifier => User.Identifier;
        public string Username => User.Username;
        #endregion
    }
}