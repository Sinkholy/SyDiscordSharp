using Gateway.Entities.Audit.LogEntry.EntryChange;
using Gateway.Entities.Audit.LogEntry.EntryOptionalInfo;
using System.Collections.Generic;

namespace Gateway.Entities.Audit.LogEntry
{
    public interface IAuditLogEntry
    {
        string TargetIdentifier { get; }
        string UserIdentifier { get; }
        string Identifier { get; }
        AuditLogEntryType EntryType { get; }
        string Reason { get; }
        IReadOnlyCollection<IEntryChange> Changes { get; }
        IOptionalEntryInfo OptionalInfo { get; }
    }
}
