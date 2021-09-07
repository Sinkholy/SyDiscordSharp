namespace DiscordDataObjects.Channels.DM
{
    public interface IGroupDMTextChannel : IDMChannel
    {
        string Icon { get; }
        string Name { get; }
    }
}