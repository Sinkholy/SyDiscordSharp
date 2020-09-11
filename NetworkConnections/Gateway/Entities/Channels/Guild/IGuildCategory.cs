using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels
{
    public interface IChannelCategory : IChannel
    {
        string GuildIdentifier { get; }
        string Name { get; }
        int Position { get; }
        bool NSFW { get; }
        IReadOnlyCollection<Overwrite> PermissionsOverwrite { get; }
    }
}
