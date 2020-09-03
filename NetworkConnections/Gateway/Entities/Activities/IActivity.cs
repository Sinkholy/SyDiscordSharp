using Gateway.Entities.Activities.Custom;
using Gateway.Entities.Activities.Gaming;
using Gateway.Entities.Activities.Listening;
using Gateway.Entities.Activities.Streaming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway.Entities.Activities
{
    [JsonConverter(typeof(IActivityConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public interface IActivity
    {
        ActivityType Type { get; }
        string Name { get; }
        DateTime CreatedAt { get; }
        ActivityFlags? Flags { get; }
    }
    internal class IActivityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IActivity);
        }
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            string activityType = json["type"].ToString();
            if (Enum.TryParse(activityType, out ActivityType type))
            {
                switch (type) // TODO: IIRC
                {
                    case ActivityType.Game:
                        return serializer.Deserialize<IGamingActivity>(json.CreateReader());
                    case ActivityType.Listening:
                        return serializer.Deserialize<IListeningActivity>(json.CreateReader());
                    case ActivityType.Custom:
                        return serializer.Deserialize<ICustomStatusActivity>(json.CreateReader());
                    case ActivityType.Streaming:
                        return serializer.Deserialize<IStreamingActivity>(json.CreateReader());
                    default:
                        return null;
                }
            }
            else
            {
                // TODO: исключение или логирование
                return null;
            }
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
