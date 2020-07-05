using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Role
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier;
        [JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonProperty(PropertyName = "color")]
        public int Color; //Hexadecimal
        [JsonProperty(PropertyName = "position")]
        public int Position;
        [JsonProperty(PropertyName = "permissions")]
        public int Permissions;
        [JsonProperty(PropertyName = "hoist")]
        public bool Hoist;
        [JsonProperty(PropertyName = "managed")]
        public bool Managed;
        [JsonProperty(PropertyName = "mentionable")]
        public bool Mentionable;
    }
}
