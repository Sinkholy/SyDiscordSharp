using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway.Entities.Message
{
    [JsonConverter(typeof(IActivityMessageConverter))]
    public interface IActivityMessage : IMessage
    {
        MessageActivity Activity { get; }
        MessageApplication Application { get; }
    }
    internal class IActivityMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IActivityMessage);
        }
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            IActivityMessage result = new ActivityMessage();
            JsonConvert.PopulateObject(json.ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
