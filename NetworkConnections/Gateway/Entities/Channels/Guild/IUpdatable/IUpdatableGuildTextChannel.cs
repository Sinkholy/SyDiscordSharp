namespace Gateway.Entities.Channels.Guild.IUpdatable
{
    public interface IUpdatableGuildTextChannel : IUpdatableGuildChannel
    {
        void SetNewCategory(string category);
        void SetNewRateLimit(int rateLimit);
        void SetNewTopic(string topic);
    }
}
