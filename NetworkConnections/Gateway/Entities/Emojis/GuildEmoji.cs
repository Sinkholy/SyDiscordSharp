using Newtonsoft.Json;

namespace Gateway.Entities.Emojis
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GuildEmoji : Emoji, IGuildEmoji
    {
        [JsonProperty(PropertyName = "id")]
        public override string Identifier { get; private protected set; }
        [JsonProperty(PropertyName = "roles")]
        public Role[] Roles { get; private set; }
        [JsonProperty(PropertyName = "require_colons")]
        public bool RequireColons { get; private set; }
        [JsonProperty(PropertyName = "managed")]
        public bool Managed { get; private set; }
        [JsonProperty(PropertyName = "available")]
        public bool Available { get; private set; }
        public string Mention => $"<:{Name}:{Identifier}>";
        public override bool IsUnicodeEmoji => false;
    }
}
