using Gateway.Enums;
using Gateway.PayloadObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway
{
    [JsonObject(MemberSerialization.OptIn)]
    class GatewayPayload
    {
        [JsonProperty(propertyName: "op", Order = 2)]
        public Opcode Opcode;
        [JsonProperty(propertyName: "d", Order = 3)]
        public IGatewayDataObject Data;
        [JsonProperty(propertyName: "s", Order = 1)]
        public string Sequence = null;
        [JsonProperty(propertyName: "t", Order = 0)]
        public string EventName = null;

        public GatewayPayload(Opcode opcode, IGatewayDataObject data, string eventName = null, string sequence = null)
        {
            this.Opcode = opcode;
            this.Data = data;
            this.Sequence = sequence;
            this.EventName = eventName;
        }
    }
}
