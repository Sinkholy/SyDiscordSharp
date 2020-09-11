using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gateway.Entities.Channels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Overwrite // TODO: интерфейс?
    {
        [JsonProperty(PropertyName = "id")]
        internal string Identifier { get; private set; }
        [JsonProperty(PropertyName = "type")]
        internal OverwriteType Type { get; private set; }
        [JsonProperty(PropertyName = "allow")]
        public int Allow { get; internal set; }
        [JsonProperty(PropertyName = "deny")]
        public int Deny { get; internal set; }
    }
    internal enum OverwriteType : byte
    {
        Role,
        Member
    }
}
