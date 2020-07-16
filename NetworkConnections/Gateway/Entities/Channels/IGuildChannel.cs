using Gateway.Entities.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels
{
    interface IGuildChannel : IChannel
    {
        string GuildIdentifier { get; }

        void UpdateChannelGuildId(IGuild guild);
    }
}
