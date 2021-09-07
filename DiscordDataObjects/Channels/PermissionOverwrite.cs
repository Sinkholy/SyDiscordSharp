using Newtonsoft.Json;

namespace DiscordDataObjects.Channels
{
    public class PermissionOverwrite // TODO: разобраться с Receiving\Sending и Permissions sets
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "type")]
        public OverwriteType Type { get; private set; }
        [JsonProperty(PropertyName = "allow")]
        public int Allow { get; internal set; }
        [JsonProperty(PropertyName = "deny")]
        public int Deny { get; internal set; }

        public PermissionOverwrite(OverwriteType type, int allow, int deny)
        {
            Type = type;
            Allow = allow;
            Deny = deny;
        }
    }
    public enum OverwriteType : byte
    {
        Role,
        Member
    }
}
