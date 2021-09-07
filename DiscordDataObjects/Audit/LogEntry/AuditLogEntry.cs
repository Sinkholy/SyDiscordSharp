using DiscordDataObjects.Audit.LogEntry.EntryChange;
using DiscordDataObjects.Audit.LogEntry.EntryOptionalInfo;

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordDataObjects.Audit.LogEntry
{
    public class AuditLogEntry : IAuditLogEntry
    {
        [JsonProperty(PropertyName = "target_id")]
        public string TargetIdentifier { get; private set; }
        [JsonProperty(PropertyName = "user_id")]
        public string UserIdentifier { get; private set; }
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "action_type")]
        public AuditLogEntryType EntryType { get; private set; }
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; private set; }
        public IReadOnlyCollection<IEntryChange> Changes => changes;
        public IOptionalEntryInfo OptionalInfo => optionalInfo;

        [JsonProperty(PropertyName = "options")]
        private OptionalEntryInfo optionalInfo;
        [JsonProperty(PropertyName = "changes")]
        private EntryChange.EntryChange[] changes;
    }
}
