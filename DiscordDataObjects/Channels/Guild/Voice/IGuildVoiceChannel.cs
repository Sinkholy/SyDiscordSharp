using System.Collections.Generic;

using DiscordDataObjects.VoiceSession;

namespace DiscordDataObjects.Channels.Guild.Voice
{
    public interface IGuildVoiceChannel : IGuildChannel
    {
        int Bitrate { get; }
        int UserLimit { get; }
        IReadOnlyCollection<IVoiceSession> ActiveVoiceSessions { get; }
        IReadOnlyCollection<string> UsersInChannel { get; }
    }
}
