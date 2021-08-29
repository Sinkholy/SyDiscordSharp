using Gateway.Entities.Audit.LogEntry;
using Gateway.Entities.Channels;
using Gateway.Entities.Integration;
using Gateway.Entities.Users;
using Gateway.Entities.Webhook;
using System.Collections.Generic;

namespace Gateway.Entities.Audit
{
    interface IAuditLog
    {
        IReadOnlyCollection<IWebhook> Webhooks { get; }
        IReadOnlyCollection<IUser> Users { get; }
        IReadOnlyCollection<IIntegration> Integrations { get; }
        IReadOnlyCollection<IAuditLogEntry> Entries { get; }
        IReadOnlyCollection<IChannel> Threads { get; }
    }
}
