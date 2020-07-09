using Gateway.DataObjects.Channels;
using Gateway.DataObjects.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Message
{
    internal interface IMessage
    {
        string Identifier { get; }
        IGuild Guild { get; }
        IChannel Channel { get; }
    }
}
