namespace Gateway.Entities.Channels.DM
{
    public interface IGroupDMTextChannel : IDMChannel
    {
        string Icon { get; }
        string Name { get; }
    }
}