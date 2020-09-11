namespace Gateway.Entities.Channels.Voice
{
    public interface IUpdatableVoiceChannel : IUpdatableGuildChannel
    {
        void SetNewBitrate(int bitrate);
        void SetNewUserLimit(int limit);
        void SetNewCategory(string category);
    }
}
