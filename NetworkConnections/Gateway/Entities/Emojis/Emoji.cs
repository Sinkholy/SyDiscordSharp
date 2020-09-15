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
        public virtual string Identifier 
        {
            get => Name;
            private protected set => Identifier = value;
        }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "animated")]
        public bool Animated { get; private set; }
        public virtual bool IsUnicodeEmoji => true;
        public virtual string Mention => Name;
        public virtual string UrlEncoded => Name;
    }
}
