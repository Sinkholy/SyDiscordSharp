using Gateway.DataObjects.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Channels
{
    interface IGuildChannel : IChannel
    {
        string GuildIdentifier { get; }

        void UpdateChannelGuildId(IGuild guild);
    }
}
