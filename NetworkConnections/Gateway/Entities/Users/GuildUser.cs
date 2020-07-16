using Gateway.Entities.Guilds;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Users
{
    internal class GuildUser : IUser
    {
        #region IUser implementation
        public bool Verified => User.Verified;
        public bool MFA => User.MFA;
        public string Identifier => User.Identifier;
        public string eMail => User.eMail;
        public string Username => User.Username;
        public string Discriminator => User.Discriminator;
        public string FullName => User.FullName;
        public bool IsBot => User.IsBot;
        public string AvatarIdentifier => User.AvatarIdentifier;
        public string Locale => User.Locale;
        public bool System => User.System;
        public PremiumType PremiumType => User.PremiumType;
        public UserFlags Flags => User.Flags;
        public UserFlags PublicFlags => User.PublicFlags;
        #endregion
        #region Public props\fields
        public IReadOnlyCollection<Role> Roles => roles as IReadOnlyCollection<Role>;
        [JsonProperty(PropertyName = "joined_at")]
        public DateTime JoinedAt { get; private set; }
        [JsonProperty(PropertyName = "premium_since")]
        public DateTime? PremiumSince { get; private set; }
        [JsonProperty(PropertyName = "nick")]
        public string Nickname { get; private set; }

        [JsonProperty(PropertyName = "deaf")]
        public bool Deafeaned { get; private set; }
        [JsonProperty(PropertyName = "mute")]
        public bool Muted { get; private set; }
        public IGuild Guild { get; set; }
        #endregion
        #region Private props\fields
        internal List<Role> roles { get; private set; }
        [JsonProperty(PropertyName = "roles")]
        internal string[] RolesIdentifiers { get; private set; }
        [JsonProperty(PropertyName = "user")]
        internal User User { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier;
        #endregion

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            if(GuildIdentifier != null)
                Guild = DiscordGatewayClient.TryToGetGuild(GuildIdentifier);
            roles = new List<Role>(capacity: RolesIdentifiers.Length);
        }
    }
}
