using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway.Entities.Activities.Gaming
{
    [JsonConverter(typeof(IGamingActivityConverter))]
    public interface IGamingActivity : IActivity
    {
        DateTime? StartedAt { get; }
    }
    internal class IGamingActivityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IGamingActivity);
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            IGamingActivity result;
            if (IsItRichPresenceActivity(json))
            {
                result = serializer.Deserialize<IRichPresenceGamingActivity>(json.CreateReader());
            }
            else
            {
                result = new GamingActivity();
                JsonConvert.PopulateObject(json.ToString(), result);
            }
            return result;
        }
        private bool IsItRichPresenceActivity(JObject json)
        {
            if (json.ContainsKey("state") 
                || json.ContainsKey("details")
                || json.ContainsKey("assets")
                || json.ContainsKey("party"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
