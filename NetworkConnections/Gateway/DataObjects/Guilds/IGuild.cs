using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Guilds
{
    public interface IGuild
    {
        string Identifier { get; }
        void UpdateGuild(IGuild newGuildInfo);
    }
}
