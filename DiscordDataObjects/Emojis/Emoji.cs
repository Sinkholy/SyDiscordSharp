using Gateway.Entities.Users;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Emojis
{
    public class Emoji : IEmoji
    {
        public virtual string Identifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "animated")]
        public bool IsAnimated { get; private set; }
        public string Mention => $"<{(IsAnimated ? "a" : string.Empty)}:{Name}:{Identifier}>";
        public bool IsUnicodeEmoji => true;
        public string UrlEncoded => $"{Name}%3A{Identifier}";
        public Role[] AvailableForRoles { get; private set; }
        public IUser CreatedBy { get; private set; }
		public bool RequireColons { get; private set; }
		public bool Managed { get; private set; }
		public bool Available { get; private set; }
	}
}
