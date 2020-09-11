namespace Gateway.Entities.Channels.Guild.IUpdatable
{
    public interface IUpdatableGuildTextChannel : IUpdatableGuildChannel
    {
        void SetNewRateLimit(int rateLimit);
        void SetNewTopic(string topic);
        void SetNewLastMessageIdentifier(string messageId);
    }
}
