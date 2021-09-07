using DiscordDataObjects.Users;
using DiscordDataObjects.Users.Activities;

using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDataObjects.Guilds.Presences
{
    public class Presence : IPresence
    {
        #region IPresence implementation
        public string UserIdentifier => user.Identifier;
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        public IReadOnlyCollection<string> UserRolesIdentifier => userRolesIdentifier;
        public IActivity VisibleActivity => _game ?? activities.FirstOrDefault(); // TAI: я вообще не уверен, что FOD возвращает правильную активити
        public IReadOnlyCollection<IActivity> Activities => activities;
        [JsonProperty(PropertyName = "premium_since")]
        public DateTime? PremiumSince { get; private set; }
        [JsonProperty(PropertyName = "status")]
        public UserStatus UserStatus { get; private set; }
        [JsonProperty(PropertyName = "client_status")]
        public ClientPlatformStatuses UserPlatformStatuses { get; private set; }
        [JsonProperty(PropertyName = "nick")]
        public string Nickname { get; private set; }
        #endregion

        [JsonProperty(PropertyName = "roles")]
        private string[] userRolesIdentifier;
        [JsonProperty(PropertyName = "activities", NullValueHandling = NullValueHandling.Ignore)]
        private IActivity[] activities;
        [JsonProperty(PropertyName = "activity", NullValueHandling = NullValueHandling.Ignore)]
        private IActivity _game;
        [JsonProperty(PropertyName = "user")]
        private User user;
    }
}
