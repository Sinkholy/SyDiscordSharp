using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Guilds
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UnavailableGuild
    {
        [JsonProperty(PropertyName = "id")]
        internal string Identifier;
        [JsonProperty(PropertyName = "available")]
        internal bool Available;
    }
}