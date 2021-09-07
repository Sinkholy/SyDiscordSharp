using Gateway.Entities.Integration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordDataObjects.Users.Connection
{
    public class Connection : IConnection
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; } // TAI: enum?
        [JsonProperty(PropertyName = "revoked")]
        public bool? IsRevoked { get; private set; }
        [JsonProperty(PropertyName = "verified")]
        public bool IsVerified { get; private set; }
        [JsonProperty(PropertyName = "friend_sync")]
        public bool IsFriendSync { get; private set; }
        [JsonProperty(PropertyName = "show_activity")]
        public bool IsShowActivity { get; private set; }
        [JsonProperty(PropertyName = "visibility")]
        public VisibilityType Visibility { get; private set; }
        public IReadOnlyCollection<IIntegration> Integrations => integrations;

        [JsonProperty(PropertyName = "integrations")]
        private Integration.Integration[] integrations;
    }
}
