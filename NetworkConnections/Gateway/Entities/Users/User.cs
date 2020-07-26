using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Users
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class User : IUser, IUpdatableUser
    {
        #region IUpdatableUser impl
        public virtual string Update(IUser newUserInfo)
        {
            StringBuilder result = new StringBuilder();
            User newUser = newUserInfo as User;
            if (Username != newUser.Username)
            {
                Username = newUser.Username;
                result.Append("Username | ");
            }
            if(Verified != newUser.Verified)
            {
                Verified = newUser.Verified;
                result.Append("Verified | ");
            }
            if(MFA != newUser.MFA)
            {
                MFA = newUser.MFA;
                result.Append("MFA | ");
            }
            if (IsBot != newUser.IsBot) //TODO : нужна ли обработка IsBot?
            {
                IsBot = newUser.IsBot;
                result.Append("IsBot | ");
            }
            if (eMail != newUser.eMail)
            {
                eMail = newUser.eMail;
                result.Append("eMail | ");
            }
            if (Discriminator != newUser.Discriminator)
            {
                Discriminator = newUser.Discriminator;
                result.Append("Discriminator | ");
            }
            if (AvatarIdentifier != newUser.AvatarIdentifier)
            {
                AvatarIdentifier = newUser.AvatarIdentifier;
                result.Append("Avatar | ");
            }
            if (PremiumType != newUser.PremiumType)
            {
                eMail = newUser.eMail;
                result.Append("Premium type | ");
            }
            if (System != newUser.System)
            {
                System = newUser.System;
                result.Append("Systen | ");
            }
            if (Locale != newUser.Locale)
            {
                Locale = newUser.Locale;
                result.Append("Locale | ");
            }
            if (Flags != newUser.Flags)
            {
                Flags = newUser.Flags;
                result.Append("Flags | ");
            }
            if (PublicFlags != newUser.PublicFlags)
            {
                PublicFlags = newUser.PublicFlags;
                result.Append("Public flags | ");
            }
            return result.ToString();
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
        #endregion
        #region Ctor's
        internal User() { }
        internal User(string id)
        {
            Identifier = id;
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
