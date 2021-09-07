using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDataObjects.Guilds.Invite
{
    public interface IInvite
    {
        string Code { get; }
        string GuildIdentifier { get; }
    }
}