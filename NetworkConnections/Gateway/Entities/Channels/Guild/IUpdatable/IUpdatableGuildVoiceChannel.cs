namespace Gateway.Entities.Channels.Guild.IUpdatable
{
    public interface IUpdatableGuildVoiceChannel : IUpdatableGuildChannel
    {
        void SetNewBitrate(int bitrate);
        void SetNewUserLimit(int limit);
    }
}
