using Gateway.Entities.VoiceSession;
using System.Collections.Generic;

namespace Gateway.Entities.Channels.Voice
{
    public interface IVoiceChannel : IChannel
    {
        int Bitrate { get; }
        int UserLimit { get; }
        string GuildIdentifier { get; }
        string Name { get; }
        bool NSFW { get; }
        int Position { get; }
        string CategoryIdentifier { get; }

        //IReadOnlyCollection<Overwrite> PermissionsOverwrite { get; }
        IReadOnlyCollection<IVoiceSession> ActiveVoiceSessions { get; }
        IReadOnlyCollection<string> UsersInChannel { get; }
    }
}
