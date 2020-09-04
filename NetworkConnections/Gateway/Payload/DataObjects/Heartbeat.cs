using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    class Heartbeat : IGatewayDataObject
    {
        [JsonProperty(PropertyName = "s")]
        public int Sequence;
        public Heartbeat() { }
    }
}
