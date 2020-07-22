using Gateway.Entities.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels
{
    interface IUpdatableChannel
    {
        string UpdateChannel(IChannel channelNewInfo);
    }
}
