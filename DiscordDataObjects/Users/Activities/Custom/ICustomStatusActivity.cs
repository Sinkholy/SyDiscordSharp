using DiscordDataObjects.Emojis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DiscordDataObjects.Users.Activities.Custom
{
    [JsonConverter(typeof(ICustomStatusActivityConverter))]

    public interface ICustomStatusActivity : IActivity
    {
        string Status { get; }
        IEmoji Emoji { get; }
    }
    internal class ICustomStatusActivityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ICustomStatusActivity);
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            CustomStatusActivity result = new CustomStatusActivity();
            JsonConvert.PopulateObject(JObject.Load(reader).ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
