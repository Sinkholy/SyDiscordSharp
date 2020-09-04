using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Resume : IGatewayDataObject
    {
        [JsonProperty(propertyName: "token", Order = 0)]
        public string Token;
        [JsonProperty(propertyName: "session_id", Order = 1)]
        public string SessionId;
        [JsonProperty(propertyName: "seq", Order = 2)]
        public int LastSequence;
        public Resume(string token, string session_id, int lastSequence)
        {
            Token = token;
            SessionId = session_id;
            LastSequence = lastSequence;
        }
    }
}
