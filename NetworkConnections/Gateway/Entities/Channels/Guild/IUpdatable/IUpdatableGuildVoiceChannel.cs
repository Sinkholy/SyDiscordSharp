namespace Gateway.Entities.Channels.Guild.IUpdatable
{
    public interface IUpdatableVoiceChannel : IUpdatableGuildChannel
    {
        void SetNewBitrate(int bitrate);
        void SetNewUserLimit(int limit);
        void SetNewCategory(string category);
    }
}
