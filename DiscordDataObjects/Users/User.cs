using Gateway.Entities.Presences;
using Gateway.Payload.DataObjects.Dispatch.DispatchEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDataObjects.Users
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class User : IUser
    {
        #region IUpdatableUser impl 
        public void UpdateAvatar(string avatarHash)// TODO: ннада?
        {
            AvatarIdentifier = avatarHash;
        }
        public void UpdateUsername(string username)
        {
            Username = username;
        }
        #endregion
        // If you wanna ask me "hey dude why this fields marked as virtual?"
        // I made this fields virtual because of descendant of this class: "GuildUser"
        // GuildUser contains field with User type which contains all information about user w\o
        // guild information. Sounds ilke a boxing right? So this information such a username, eMail etc.
        // in GuildUser contains in User typed field.
        #region IUser implementation
        [JsonProperty(PropertyName = "username")]
        public virtual string Username { get; private set; }
        [JsonProperty(PropertyName = "verified")]
        public virtual bool Verified { get; private set; }
        [JsonProperty(PropertyName = "mfa_enabled")]
        public virtual bool MFA { get; private set; }
        [JsonProperty(PropertyName = "id")]
        public virtual string Identifier { get; private set; }
        [JsonProperty(PropertyName = "bot")]
        public virtual bool IsBot { get; private set; }
        [JsonProperty(PropertyName = "email")]
        public virtual string eMail { get; private set; }
        [JsonProperty(PropertyName = "discriminator")]
        public virtual string Discriminator { get; private set; }
        [JsonProperty(PropertyName = "avatar")]
        public virtual string AvatarIdentifier { get; private set; }
        public virtual string FullName => Username + "#" + Discriminator;
        [JsonProperty(PropertyName = "premium_type")]
        public virtual PremiumType PremiumType { get; private set; }
        [JsonProperty(PropertyName = "system")]
        public virtual bool System { get; private set; }
        [JsonProperty(PropertyName = "locale")]
        public virtual string Locale { get; private set; }
        [JsonProperty(PropertyName = "flags")]
        public virtual UserFlags Flags { get; private set; }
        [JsonProperty(PropertyName = "public_flags")]
        public virtual UserFlags PublicFlags { get; private set; }
        public string Mention => $"<@!{Identifier}>";
		#endregion
		#region Ctor's
		internal User() { }
        internal User(string username)
        {

        }
        internal User(bool verified,
                    string username,
                    bool mfa,
                    string identifier,
                    string eMail,
                    string discriminator,
                    bool bot,
                    string avatarId)
        {
            Verified = verified;
            Username = username;
            MFA = mfa;
            Identifier = identifier;
            this.eMail = eMail;
            Discriminator = discriminator;
            IsBot = bot;
            AvatarIdentifier = avatarId;
        }
        #endregion
    }
}
