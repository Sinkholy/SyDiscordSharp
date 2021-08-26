using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway.Entities.Activities.Streaming
{
    [JsonConverter(typeof(IStreamingActivityConverter))]
    public interface IStreamingActivity : IActivity
    {
        Uri Url { get; }
        StreamingPlatform Platform { get; }
        string StreamName { get; }
        string Game { get; }
        string StreamImagePreview { get; }
    }
    public enum StreamingPlatform : byte
    {
        Twitch,
        Youtube
    }
    internal class IStreamingActivityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IStreamingActivity);
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            StreamingActivity result = new StreamingActivity();
            JsonConvert.PopulateObject(JObject.Load(reader).ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
