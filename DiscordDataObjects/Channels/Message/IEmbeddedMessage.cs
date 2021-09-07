using DiscordDataObjects.Channels.Message.Embed;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DiscordDataObjects.Channels.Message
{
    [JsonConverter(typeof(IEmbeddedMessageConverter))]
    public interface IEmbeddedMessage : IMessage
    {
        EmbedData[] Embeds { get; }
    }
    internal class IEmbeddedMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IEmbeddedMessage);
        }
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            IEmbeddedMessage result = new EmbeddedMessage();
            JsonConvert.PopulateObject(json.ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
