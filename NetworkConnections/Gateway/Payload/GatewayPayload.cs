using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway
{
    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(IGatewayPayloadConverter))]
    internal class GatewayPayload
    {
        [JsonProperty(propertyName: "op", Order = 2)]
        internal Opcode Opcode { get; private set; }
        [JsonProperty(propertyName: "d", Order = 3)]
        internal string Data { get; private set; }
        [JsonProperty(propertyName: "s", Order = 1)]
        internal int? Sequence { get; private set; }
        [JsonProperty(propertyName: "t", Order = 0)]
        internal string EventName { get; private set; }
        internal GatewayPayload(Opcode opcode, string data, int? sequence, string eventName = null)
        {
            Opcode = opcode;
            Data = data;
            Sequence = sequence;
            EventName = eventName;
        }

        private class IGatewayPayloadConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(GatewayPayload);
            public override bool CanRead => true;
            public override bool CanWrite => false;
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject json = JObject.Load(reader);
                string data = json["d"].ToString(),
                       opcode = json["op"].ToString(),
                       sequence = json["s"].ToString(),
                       eventName = json["t"].ToString();
                Opcode opcodeParsed = ParseOpcode(opcode);
                int.TryParse(sequence, out int sequenceParsed);
                return new GatewayPayload(opcodeParsed, data, sequenceParsed, eventName);
            }
            private Opcode ParseOpcode(string input)
            {
                Opcode result = Opcode.UnknownOpcode;
                bool converted = Enum.TryParse(input, out Opcode parsed);
				if (converted)
				{
                    result = parsed;
				}
                return result;
            }
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
