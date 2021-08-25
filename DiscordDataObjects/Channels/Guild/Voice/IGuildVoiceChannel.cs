using Gateway.Entities.VoiceSession;
using System.Collections.Generic;

namespace Gateway.Entities.Channels.Guild.Voice
{
    public interface IGuildVoiceChannel : IGuildChannel
    {
        int Bitrate { get; }
        int UserLimit { get; }
        IReadOnlyCollection<IVoiceSession> ActiveVoiceSessions { get; }
        IReadOnlyCollection<string> UsersInChannel { get; }
    }
}
