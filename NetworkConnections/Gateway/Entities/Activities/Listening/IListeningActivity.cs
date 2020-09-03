using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway.Entities.Activities.Listening
{
    [JsonConverter(typeof(IListeningActivityConverter))]
    public interface IListeningActivity : IActivity
    {
        string Band { get; }
        string Song { get; }
        string Album { get; }
        DateTime? StartedAt { get; }
        DateTime? EndedAt { get; }
    }
    internal class IListeningActivityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IListeningActivity);
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ListeningActivity result = new ListeningActivity();
            JsonConvert.PopulateObject(JObject.Load(reader).ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
