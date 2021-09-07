using Gateway.Entities.Audit.LogEntry;
using Gateway.Entities.Channels;
using Gateway.Entities.Integration;
using Gateway.Entities.Users;
using Gateway.Entities.Webhook;
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
        private Integration.Integration[] integrations;
        [JsonProperty(PropertyName = "webhooks")]
        private Webhook.Webhook[] webhooks;
        [JsonProperty(PropertyName = "users")]
        private User[] users;
        private IChannel[] threads;
    }
}
