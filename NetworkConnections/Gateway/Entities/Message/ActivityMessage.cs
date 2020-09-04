using Newtonsoft.Json;

namespace Gateway.Entities.Message
{
    internal class ActivityMessage : Message, IActivityMessage
    {
        [JsonProperty(PropertyName = "activity")]
        public MessageActivity Activity { get; private set; }
        [JsonProperty(PropertyName = "application")]
        public MessageApplication Application { get; private set; }

        public ActivityMessage(string content,
                               string nonce,
                               bool tts = false,
                               MessageAttachment[] attachements = null)
            : base(content, nonce, tts, attachements)
        {

        }
        public ActivityMessage() { }
    }
    public class MessageActivity
    {
        [JsonProperty(PropertyName = "type")]
        public MessageActivityType Type { get; private set; }
        [JsonProperty(PropertyName = "party_id")]
        public string PartyId { get; private set; }
        public enum MessageActivityType : byte
        {
            Join,
            Spectrate,
            Listen,
            JoinRequest = 5
        }
    }
    public class MessageApplication
    {
        [JsonProperty(PropertyName = "id")]
        internal string Identifier;
        [JsonProperty(PropertyName = "cover_image")]
        internal string CoverImage;
        [JsonProperty(PropertyName = "description")]
        internal string Description;
        [JsonProperty(PropertyName = "icon")]
        internal string Icon;
        [JsonProperty(PropertyName = "name")]
        internal string Name;
    }
}
