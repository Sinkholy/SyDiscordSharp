using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Users
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class User : IUser
    {
        #region IUser implementation
        [JsonProperty(PropertyName = "username")]
        public string Username { get; private set; }
        [JsonProperty(PropertyName = "verified")]
        public bool Verified { get; private set; }
        [JsonProperty(PropertyName = "mfa_enabled")]
        public bool MFA { get; private set; }
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "bot")]
        public bool IsBot { get; private set; }
        [JsonProperty(PropertyName = "email")]
        public string eMail { get; private set; }
        [JsonProperty(PropertyName = "discriminator")]
        public string Discriminator { get; private set; }
        [JsonProperty(PropertyName = "avatar")]
        public string AvatarIdentifier { get; private set; }
        public string FullName => Username + "#" + Discriminator;        
        [JsonProperty(PropertyName = "premium_type")]
        public PremiumType PremiumType { get; private set; }
        [JsonProperty(PropertyName = "system")]
        public bool System { get; private set; }
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; private set; }
        [JsonProperty(PropertyName = "flags")]
        public UserFlags Flags { get; private set; }
        [JsonProperty(PropertyName = "public_flags")]
        public UserFlags PublicFlags { get; private set; }
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
