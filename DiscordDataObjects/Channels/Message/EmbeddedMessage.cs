using DiscordDataObjects.Channels.Message.Embed;

using Newtonsoft.Json;

namespace DiscordDataObjects.Channels.Message
{
    internal class EmbeddedMessage : Message, IEmbeddedMessage 
    {
        [JsonProperty(PropertyName = "embeds")]
        public EmbedData[] Embeds { get; private set; }

        public EmbeddedMessage(string content,
                               string nonce,
                               EmbedData[] embeds,
                               bool tts = false,
                               MessageAttachment[] attachements = null)
            : base(content, nonce, tts, attachements)
        {
            Embeds = embeds;
        }
        public EmbeddedMessage() { }
    }
}
