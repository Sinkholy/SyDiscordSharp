namespace DiscordDataObjects.Channels.Guild.IUpdatable
{
    public interface IUpdatableGuildVoiceChannel : IUpdatableGuildChannel
    {
        //
        // Summary:
        //     Sets channel's new bitrate.
        //
        // Parameters:
        //     bitrate:
        //          New bitrate which will be set to channel.
        //          Must be greater or equal to 8000 kbps. Currently has limits by server boost:
        //          Level0: 96000kbps
        //          Level1: 128000kbps
        //          Level2: 256000kbps
        //          Level3: 384000kbps
        // Exceptions:
        //     T:System.ArgumentOutOfRangeException:
        //          Bitrate is less then 8000 kbps.
        void SetNewBitrate(int bitrate);
        //
        // Summary:
        //     Sets new voice channel users limit.
        //
        // Parameters:
        //     limit:
        //          New users limit which will be set to channel.
        //          Must be in range 0 - 99.
        //          0 refers to no limit.
        // Exceptions:
        //     T:System.ArgumentOutOfRangeException:
        //          Limit is less then 0 or greater then 99.
        void SetNewUserLimit(int limit);
    }
}
