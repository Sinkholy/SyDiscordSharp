using Newtonsoft.Json;

namespace DiscordDataObjects.Audit.LogEntry.EntryOptionalInfo
{
    public class OptionalEntryInfo : IOptionalEntryInfo
    {
        [JsonProperty(PropertyName = "delete_member_days")]
        public int PrunedMembersDaysCount { get; private set; }
        [JsonProperty(PropertyName = "members_removed")]
        public int PrunedMembersCount { get; private set; }
        [JsonProperty(PropertyName = "channel_id")]
        public string ChannelIdentifier { get; private set; }
        [JsonProperty(PropertyName = "message_id")]
        public string MessageIdentifier { get; private set; }
        [JsonProperty(PropertyName = "count")]
        public int EntitiesCount { get; private set; }
        [JsonProperty(PropertyName = "overwritten entity")]
        public string OverwrittenEntityIdentifier { get; private set; }
        [JsonProperty(PropertyName = "type")]
        public string OverwrittenEntityType { get; private set; }
        [JsonProperty(PropertyName = "role_name")]
        public string RoleName { get; private set; }
    }
}
