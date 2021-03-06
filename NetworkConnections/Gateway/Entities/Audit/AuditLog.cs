﻿using Gateway.Entities.Audit.LogEntry;
using Gateway.Entities.Integration;
using Gateway.Entities.Users;
using Gateway.Entities.Webhook;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gateway.Entities.Audit
{
    public class AuditLog : IAuditLog
    {
        public IReadOnlyCollection<IWebhook> Webhooks => webhooks;
        public IReadOnlyCollection<IUser> Users => users;
        public IReadOnlyCollection<IIntegration> Integrations => integrations;
        public IReadOnlyCollection<IAuditLogEntry> Entries => entries;

        [JsonProperty(PropertyName = "audit_log_entries")]
        private AuditLogEntry[] entries;
        [JsonProperty(PropertyName = "integrations")]
        private Integration.Integration[] integrations;
        [JsonProperty(PropertyName = "webhooks")]
        private Webhook.Webhook[] webhooks;
        [JsonProperty(PropertyName = "users")]
        private User[] users;
    }
}
