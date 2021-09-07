using DiscordDataObjects.Audit.LogEntry;
using DiscordDataObjects.Channels;
using DiscordDataObjects.Guilds.Integration;
using DiscordDataObjects.Users;
using DiscordDataObjects.Webhook;

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordDataObjects.Audit
{
    public class AuditLog : IAuditLog
    {
        public IReadOnlyCollection<IWebhook> Webhooks => webhooks;
        public IReadOnlyCollection<IUser> Users => users;
        public IReadOnlyCollection<IIntegration> Integrations => integrations;
        public IReadOnlyCollection<IAuditLogEntry> Entries => entries;
        public IReadOnlyCollection<IChannel> Threads => threads;

        [JsonProperty(PropertyName = "audit_log_entries")]
        private AuditLogEntry[] entries;
        [JsonProperty(PropertyName = "integrations")]
        private Integration[] integrations;
        [JsonProperty(PropertyName = "webhooks")]
        private Webhook.Webhook[] webhooks;
        [JsonProperty(PropertyName = "users")]
        private User[] users;
        private IChannel[] threads;
    }
}
