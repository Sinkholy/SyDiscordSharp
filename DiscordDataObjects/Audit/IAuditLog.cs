using System.Collections.Generic;

using DiscordDataObjects.Audit.LogEntry;
using DiscordDataObjects.Channels;
using DiscordDataObjects.Guilds.Integration;
using DiscordDataObjects.Users;
using DiscordDataObjects.Webhook;

namespace DiscordDataObjects.Audit
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
