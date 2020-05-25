using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.PayloadObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    class Hello
    {
        [JsonProperty(PropertyName = "heartbet_interval", Order = 0)]
        public TimeSpan HeartbeatInterval;
    }
}
