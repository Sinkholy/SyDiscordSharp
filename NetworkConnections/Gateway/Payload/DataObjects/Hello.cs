using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    class Hello : IGatewayDataObject
    {
        [JsonProperty(PropertyName = "heartbeat_interval", Order = 0)]
        public TimeSpan HeartbeatInterval;
        [JsonProperty(PropertyName = "_trace", Order = 1)]
        public object Trace;
        [JsonConstructor]
        public Hello(int heartbeat_interval, object _trace)
        {
            HeartbeatInterval = TimeSpan.FromMilliseconds(heartbeat_interval);
            Trace = _trace;
        }
    }
}
