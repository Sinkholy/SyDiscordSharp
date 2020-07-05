using Gateway.DataObjects.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Voice
{
    internal class VoiceState
    {
        string GuildIdentifier,
               ChannelIdentifier,
               UserIdentifier,
               SessionIdentifier;
        GuildUser GuildUser;
        bool Deafened,
             Muted,
             SelfDeafened,
             SelfMuted,
             SelfStream,
             Suppress;
    }
}
