using Newtonsoft.Json;

namespace Gateway.Entities.Message
{
    internal class CrossPostedMessage : Message, ICrossPostedMessage
    {
        [JsonProperty(PropertyName = "message_reference")]
        public MessageReference MessageReference { get; private set; }

        public CrossPostedMessage(string content,
                                  string nonce,
                                  bool tts = false,
                                  MessageAttachment[] attachements = null)
            : base(content, nonce, tts, attachements)
        {

        }
        public CrossPostedMessage() { }
    }
    public class MessageReference
    {
        [JsonProperty(PropertyName = "message_id")]
        private string messageIdentifier;
        [JsonProperty(PropertyName = "channel_id")]
        private string channelIdentifier;
        [JsonProperty(PropertyName = "guild_id")]
        private string guildIdentifier;
    }
}
