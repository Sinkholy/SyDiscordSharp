using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway.Entities.Activities.Gaming
{
    [JsonConverter(typeof(IRichPresenceGamingActivityConverter))]
    public interface IRichPresenceGamingActivity : IGamingActivity
    {
        string State { get; }
        int? PartySize { get; }
        int? PartySizeLimit { get; }
        string PartyIdentifier { get; }
        string Details { get; }
        string ImageName { get; }
        string ImageIdentifier { get; }
    }
    internal class IRichPresenceGamingActivityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IRichPresenceGamingActivity);
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: имплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            RichPresenceGamingActivity result = new RichPresenceGamingActivity();
            JsonConvert.PopulateObject(JObject.Load(reader).ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
