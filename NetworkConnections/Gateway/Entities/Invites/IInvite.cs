﻿using Gateway.Entities.Channels;
using Gateway.Entities.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Invite
{
    public interface IInvite
    {
        string Code { get; }
        string GuildIdentifier { get; }
    }
}