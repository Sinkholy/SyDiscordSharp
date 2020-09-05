using Gateway.Entities.Presences;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gateway.Entities.Users
{
    internal class GuildUser : User, IGuildUser, IUpdatableGuildUser
    {
        #region IUpdatableGuildUser impl
        void IUpdatableGuildUser.UpdateDeafenedState(bool deafened)
        {
            Deafeaned = deafened;
        }
        void IUpdatableGuildUser.UpdateMutedState(bool muted)
        {
            Muted = muted;
        }
        void IUpdatableGuildUser.UpdateSelfDeafenedState(bool selfDeafened)
        {
            SelfDeafened = selfDeafened;
        }
        void IUpdatableGuildUser.UpdateSelfMutedState(bool selfMuted)
        {
            SelfMuted = selfMuted;
        }
        void IUpdatableGuildUser.UpdateSelfStreamState(bool selfStream)
        {
            SelfStream = selfStream;
        }
        void IUpdatableGuildUser.UpdateSelfVideoState(bool selfVideo)
        {
            SelfVideo = selfVideo;
        }
        void IUpdatableGuildUser.UpdatePresencesEnumerable(IEnumerable<IPresence> enumerable)
        {
            presenceEnumerable = enumerable;
        }
        void IUpdatableGuildUser.UpdateRolesEnumerable(IEnumerable<Role> enumerable)
        {
            rolesEnumerable = enumerable;
        }
        void IUpdatableGuildUser.UpdateGuildIdentifier(string guildId)
        {
            GuildIdentifier = guildId;
        }
        #endregion
        #region IUser implementation
        public override bool Verified => user.Verified;
        public override bool MFA => user.MFA;
        public override string Identifier => user.Identifier;
        public override string eMail => user.eMail;
        public override string Username => user.Username;
        public override string Discriminator => user.Discriminator;
        public override string FullName => user.FullName;
        public override bool IsBot => user.IsBot;
        public override string AvatarIdentifier => user.AvatarIdentifier;
        public override string Locale => user.Locale;
        public override bool System => user.System;
        public override PremiumType PremiumType => user.PremiumType;
        public override UserFlags Flags => user.Flags;
        public override UserFlags PublicFlags => user.PublicFlags;
        #endregion
        #region IGuildUser implementation
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        public IReadOnlyCollection<Role> Roles => rolesEnumerable.ToList();
        [JsonProperty(PropertyName = "joined_at")]
        public DateTime JoinedAt { get; private set; }
        [JsonProperty(PropertyName = "premium_since")]
        public DateTime? PremiumSince { get; private set; }
        [JsonProperty(PropertyName = "nick")]
        public string Nickname { get; private set; } 
        public bool SelfDeafened { get; private set; }
        public bool SelfMuted { get; private set; }
        public bool SelfVideo { get; private set; }
        public bool SelfStream { get; private set; }
        [JsonProperty(PropertyName = "deaf")]
        public bool Deafeaned { get; private set; }
        [JsonProperty(PropertyName = "mute")]
        public bool Muted { get; private set; }
        public IPresence Presence => presenceEnumerable.FirstOrDefault();
        #endregion
        #region Private props\fields
        private IEnumerable<Role> rolesEnumerable;
        [JsonProperty(PropertyName = "roles")]
        internal string[] RolesIdentifiers { get; private set; }
        [JsonProperty(PropertyName = "user")]
        private User user;
        private IEnumerable<IPresence> presenceEnumerable;
        #endregion
    }
}
