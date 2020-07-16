using Gateway.Entities.Channels;
using Gateway.Entities.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Invite
{
    internal interface IInvite
    {
        string Code { get; }
        IGuild Guild { get; }
        IChannel Channel { get; }
    }
}
