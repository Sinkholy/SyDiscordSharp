using System;

namespace Gateway.Entities.Audit.LogEntry.EntryChange
{
    public interface IEntryChange
    {
        LogEntryChangeKey ChangeType { get; }
        object OldValueUntyped { get; }
        object NewValueUntyped { get; }
        EntryDataType DataType { get; }
    }
}
