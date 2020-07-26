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
    internal class GuildUser : User
    {
        #region IUpdatableUser impl
        public override string Update(IUser newUserInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.Update(newUserInfo));
            GuildUser newUser = newUserInfo as GuildUser;
            if(PremiumSince != newUser.PremiumSince)
            {
                PremiumSince = newUser.PremiumSince;
                result.Append("PremiumSince | ");
            }
            if(Nickname != newUser.Nickname)
            {
                Nickname = newUser.Nickname;
                result.Append("Nickname | ");
            }
            if(Deafeaned != newUser.Deafeaned)
            {
                Deafeaned = newUser.Deafeaned;
                result.Append("Deafened | ");
            }
            if (Muted != newUser.Muted)
            {
                Muted = newUser.Muted;
                result.Append("Muted | ");
            }
            if (Deafeaned != newUser.Deafeaned)
            {
                Deafeaned = newUser.Deafeaned;
                result.Append("Deafened ");
            }
            if (RolesIdentifiers.Count != newUser.RolesIdentifiers.Count)
            {
                if (RolesIdentifiers.Count > newUser.RolesIdentifiers.Count)
                {
                    int count = RolesIdentifiers.Count - newUser.RolesIdentifiers.Count;
                    result.Append($"{count} Role(s) removed | ");
                    RolesIdentifiers.RemoveRange(RolesIdentifiers.Count - count, count);
                }
                else
                {
                    int count = newUser.RolesIdentifiers.Count - RolesIdentifiers.Count;
                    result.Append($"{count} Role(s) added | ");
                    RolesIdentifiers.AddRange(newUser.RolesIdentifiers.GetRange(RolesIdentifiers.Count, count));
                }
            }
            else
            {
                for (int i = 0; i < RolesIdentifiers.Count; i++)
                {
                    if (RolesIdentifiers[i] != newUser.RolesIdentifiers[i])
                    {
                        result.Append("Role replaced | ");
                        RolesIdentifiers[i] = newUser.RolesIdentifiers[i];
                    }
                }
            }
            return result.ToString();
        }
        #endregion
        #region IUser implementation
        public override bool Verified => User.Verified;
        public override bool MFA => User.MFA;
        public override string Identifier => User.Identifier;
        public override string eMail => User.eMail;
        public override string Username => User.Username;
        public override string Discriminator => User.Discriminator;
        public override string FullName => User.FullName;
        public override bool IsBot => User.IsBot;
        public override string AvatarIdentifier => User.AvatarIdentifier;
        public override string Locale => User.Locale;
        public override bool System => User.System;
        public override PremiumType PremiumType => User.PremiumType;
        public override UserFlags Flags => User.Flags;
        public override UserFlags PublicFlags => User.PublicFlags;
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
        internal List<string> RolesIdentifiers { get; private set; }
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
            roles = new List<Role>(capacity: RolesIdentifiers.Count);
        }
    }
}
