using Gateway.Entities.Users;
using Newtonsoft.Json;
using System;

namespace DiscordDataObjects.Guilds.Integration
{
    public class Integration : IIntegration
    {

        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; } // TAI: вычислить все типы и вынести их в энум
        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; private set; }
        [JsonProperty(PropertyName = "syncing")]
        public bool Syncing { get; private set; }
        [JsonProperty(PropertyName = "role_id")]
        public string RoleIdentifier { get; private set; }
        [JsonProperty(PropertyName = "enable_emoticons")]
        public bool? IsEmoticonsEnabled { get; private set; }
        [JsonProperty(PropertyName = "expire_behavior")]
        public IntegrationExpireBehavior ExpireBehavior { get; private set; }
        [JsonProperty(PropertyName = "expire_grace_period")]
        public int ExpireGracePeriod { get; private set; }
        [JsonProperty(PropertyName = "synced_at")]
        public DateTime SyncedAt { get; private set; }
        public IIntegrationAccount Account => account;
        public IUser User => user;

        [JsonProperty(PropertyName = "account")]
        private IntegrationAccount account;
        [JsonProperty(PropertyName = "user")]
        private User user;

    }
    public enum IntegrationExpireBehavior : byte
    {
        RemoveRole,
        Kick
    }
}
