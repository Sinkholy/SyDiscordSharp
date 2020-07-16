using Gateway.DataObjects.Channels;
using Gateway.DataObjects.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Invite
{
    internal interface IInvite
    {
        string Code { get; }
        IGuild Guild { get; }
        IChannel Channel { get; }
    }
}
