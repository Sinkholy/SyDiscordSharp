namespace DiscordDataObjects.Channels.Guild.IUpdatable
{
    public interface IUpdatableGuildTextChannel : IUpdatableGuildChannel
    {
        //
        // Summary:
        //     Sets channel's new rate limit.
        //
        // Parameters:
        //     rateLimit:
        //          New amount of seconds a user has to wait before sending another message to channel.
        //          Must be in range 0 - 21600.
        // Exceptions:
        //     T:System.ArgumentOutOfRangeException:
        //          Limit is less then 0 or greater then 21600.
        void SetNewRateLimit(int rateLimit);
        //
        // Summary:
        //     Sets channel's new topic.
        //
        // Parameters:
        //     topic:
        //          Channel's new topic. String length must be less then 1024 symbols.
        // Exceptions:
        //     T:System.ArgumentOutOfRangeException:
        //          Topic is greater then 1024 characters.
        void SetNewTopic(string topic);
        //
        // Summary:
        //     Sets channel's new last message identifier.
        //
        // Parameters:
        //     messageId:
        //          New last message identifier. Must be greater then 0.
        // Exceptions:
        //     T:System.ArgumentOutOfRangeException:
        //          New message id less then 0.
        void SetNewLastMessageIdentifier(string messageId);
    }
}
