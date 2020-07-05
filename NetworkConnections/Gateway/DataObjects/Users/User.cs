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
        [JsonProperty(PropertyName = "verified")]
        internal bool Verified { get; private set; }
        [JsonProperty(PropertyName = "username")]
        public string Username { get; private set; }
        [JsonProperty(PropertyName = "mfa_enabled")]
        internal bool MFA { get; private set; }
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "email")]
        internal string eMail { get; private set; }
        [JsonProperty(PropertyName = "discriminator")]
        internal string Discriminator { get; private set; }
        [JsonProperty(PropertyName = "bot")]
        internal bool IsBot { get; private set; }
        [JsonProperty(PropertyName = "avatar")]
        internal string AvatarHash { get; private set; }
        [JsonProperty(PropertyName = "system")]
        internal bool System { get; private set; }
        [JsonProperty(PropertyName = "locale")]
        internal string Locale { get; private set; }
        [JsonProperty(PropertyName = "flags")]
        internal UserFlags Flags { get; private set; }
        [JsonProperty(PropertyName = "public_flags")]
        internal UserFlags PublicFlags { get; private set; }
        [JsonProperty(PropertyName = "premium_type")]
        internal PremiumType PremiumType { get; private set; }



        internal User() { }
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
            AvatarHash = avatarId;
        }
    }
    [Flags]
    internal enum UserFlags
    {
        None                 = 0,
        DiscordEmployee      = 1 << 0,
        DiscordPartner       = 1 << 1,
        HypeSquadEvents      = 1 << 2,
        BugHunterLevel1      = 1 << 3,
        HouseBravery         = 1 << 6,
        HouseBrilliance      = 1 << 7,
        HouseBalance         = 1 << 8,
        EarlySupporter       = 1 << 9,
        TeamUser             = 1 << 10,
        System               = 1 << 12,
        BugHunterLevel2      = 1 << 14,
        VerifiedBot          = 1 << 16,
        VerifiedBotDeveloper = 1 << 17
    }
    internal enum PremiumType
    {
        None = 0,
        NitroClassic = 1,
        Nitro = 2
    }
}
