using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gateway.DataObjects.Channels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Overwrite
    {
        [JsonProperty(PropertyName = "id")]
        string Identifier;
        [JsonProperty(PropertyName = "type")]
        OverwriteType Type; //Role or member
        [JsonProperty(PropertyName = "allow")]
        int Allow;
        [JsonProperty(PropertyName = "deny")]
        int Deny;
    }
    internal enum OverwriteType : byte
    {
        Role,
        Member
    }
}
