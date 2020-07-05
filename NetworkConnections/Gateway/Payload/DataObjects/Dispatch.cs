﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects
{
    [JsonObject(MemberSerialization.OptOut)]
    [JsonConverter(typeof(GatewayDispatchPayloadConverter))]
    internal class Dispatch : IGatewayDataObject
    {
        internal string EventData;
        internal Dispatch(string value) => EventData = value;
        private class GatewayDispatchPayloadConverter : JsonConverter //TODO : выглядит как костыль это стоит убрать и придумать 
                                                                      //другой механизм парсинга Dispatch объекта
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(IGatewayDataObject);
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
