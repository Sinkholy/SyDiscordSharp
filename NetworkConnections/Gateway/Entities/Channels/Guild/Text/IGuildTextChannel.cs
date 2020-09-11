namespace Gateway.Entities.Channels.Guild.Text
{
    public interface IGuildTextChannel : IGuildChannel
    {
        string LastMessageIdentifier { get; }
        string Topic { get; }
        int RateLimitPerUser { get; }
    }
}
