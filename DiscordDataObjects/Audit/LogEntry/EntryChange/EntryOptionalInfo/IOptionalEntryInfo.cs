namespace Gateway.Entities.Audit.LogEntry.EntryOptionalInfo
{
    public interface IOptionalEntryInfo
    {
        int PrunedMembersDaysCount { get; }
        int PrunedMembersCount { get; }
        string ChannelIdentifier { get; }
        string MessageIdentifier { get; }
        int EntitiesCount { get; }
        string OverwrittenEntityIdentifier { get; }
        string OverwrittenEntityType { get; }
        string RoleName { get; }
    }
}
