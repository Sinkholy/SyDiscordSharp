using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DiscordDataObjects.Channels.Message
{
    public interface ICrossPostedMessage : IMessage
    {
        MessageReference MessageReference { get; }
    }
    internal class ICrossPostedMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ICrossPostedMessage);
        }
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            ICrossPostedMessage result = new CrossPostedMessage();
            JsonConvert.PopulateObject(json.ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
