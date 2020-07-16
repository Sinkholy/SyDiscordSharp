using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities
{
    public class Role
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "color")]
        public int Color { get; private set; } //Hexadecimal
        [JsonProperty(PropertyName = "position")]
        public int Position { get; private set; }
        [JsonProperty(PropertyName = "permissions")]
        public int Permissions { get; private set; }
        [JsonProperty(PropertyName = "hoist")]
        public bool Hoist { get; private set; }
        [JsonProperty(PropertyName = "managed")]
        public bool Managed { get; private set; }
        [JsonProperty(PropertyName = "mentionable")]
        public bool Mentionable { get; private set; }
    }
}
