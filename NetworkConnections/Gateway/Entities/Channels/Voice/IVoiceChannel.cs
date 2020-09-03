using Gateway.Entities.VoiceSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Voice
{
    interface IVoiceChannel
    {
        void JoinChannel();
        IReadOnlyCollection<IVoiceSession> ActiveVoiceSessions { get; }
        IReadOnlyCollection<string> UsersInChannel { get; }
    }
}
