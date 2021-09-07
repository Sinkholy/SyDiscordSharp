using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway.Payload.DataObjects.Dispatch
{
    [JsonConverter(typeof(GatewayDispatchPayloadConverter))]
    internal class Dispatch
    {
        internal string EventData;
        internal Dispatch(string value) => EventData = value;
        private class GatewayDispatchPayloadConverter : JsonConverter //TODO : выглядит как костыль это стоит убрать и придумать 
                                                                      //другой механизм парсинга Dispatch объекта
        {
            public override bool CanConvert(Type objectType) => objectType == typeof();
            public override bool CanRead => true;
            public override bool CanWrite => false;
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => new Dispatch(JObject.Load(reader).ToString());
            /// <summary>
            /// Not implemented
            /// </summary>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
