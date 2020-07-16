using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Emojis
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GuildEmoji : Emoji
    {
        [JsonProperty(PropertyName = "roles")]
        public Role[] Roles;
        [JsonProperty(PropertyName = "require_colons")]
        public bool RequireColons;
        [JsonProperty(PropertyName = "managed")]
        public bool Managed;    
        [JsonProperty(PropertyName = "available")]
        public bool Available;
    }
}
