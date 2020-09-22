namespace Gateway.Entities.Audit.LogEntry.EntryChange
{
    public enum EntryDataType : byte
    {
        Integer,
        String,
        IRoleArray,
        IPermissionsOverwriteArray,
        Bool
    }
}
