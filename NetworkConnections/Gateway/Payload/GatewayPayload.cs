using Gateway.Payload.DataObjects;
using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway
{
    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(IGatewayPayloadConverter))]
    public class GatewayPayload
    {
        [JsonProperty(propertyName: "op", Order = 2)]
        public Opcode Opcode;
        [JsonProperty(propertyName: "d", Order = 3)]
        public IGatewayDataObject Data;
        [JsonProperty(propertyName: "s", Order = 1)]
        public string Sequence = null;
        [JsonProperty(propertyName: "t", Order = 0)]
        public string EventName = null;
        public GatewayPayload(Opcode opcode, IGatewayDataObject data, string eventName = null, string sequence = null)
        {
            this.Opcode = opcode;
            this.Data = data;
            this.Sequence = sequence == "" ? null : sequence;
            this.EventName = eventName == "" ? null : eventName;
        }

        private class IGatewayPayloadConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(IGatewayDataObject);
            public override bool CanRead => true;
            public override bool CanWrite => false;
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject json = JObject.Load(reader);
                string dataJson = json["d"].ToString(),
                       opcodeJson = json["op"].ToString(),
                       sequenceJson = json["s"].ToString(),
                       eventNameJson = json["t"].ToString(),
                       dataTypeFullName;
                Opcode opcode = ParseOpcode(opcodeJson);
                if(opcode == Opcode.Dispatch)
                {
                    dataTypeFullName = $"{objectType.Namespace}.Payload.DataObjects.Dispatch.{opcode}";
                }
                else
                {
                    dataTypeFullName = $"{objectType.Namespace}.Payload.DataObjects.{opcode}";
                }
                Type dataType = objectType.Assembly.GetType(dataTypeFullName);
                IGatewayDataObject dataObject = JsonConvert.DeserializeObject(dataJson, dataType) as IGatewayDataObject;

                return new GatewayPayload(opcode, dataObject, eventNameJson, sequenceJson);
            }
            private Opcode ParseOpcode(string opcodeJson)
            {
                byte opcode;
                try
                {
                    opcode = byte.Parse(opcodeJson);
                }
                catch
                {
                    throw new Exception("cannot parse opcode"); // TODO : исключение
                }

                byte[] enumValues = (byte[])Enum.GetValues(typeof(Opcode));
                bool opcodeFounded = false;
                for (byte i = 0; i < enumValues.Length; i++)
                {
                    byte current = enumValues[i];
                    if (opcode == current)
                    {
                        opcodeFounded = true;
                        opcode = current;
                        break;
                    }
                    else
                        continue;
                }
                if (opcodeFounded)
                {
                    return (Opcode)opcode;
                }
                else
                {
                    throw new Exception("unknown opcode"); //TODO : исключение
                }
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
