using Gateway.PayloadCommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Identify : IGatewayDataObject
    {
        [JsonProperty(propertyName: "token", Order = 0)]
        public string Token;
        //[JsonProperty(propertyName: "compress", Order = 2)]
        //public bool Compress;
        //[JsonProperty(propertyName: "largeThreshold", Order = 3)]
        //public int LargeThreshold;
        //[JsonProperty(propertyName: "guildSubscriptions", Order = 4)]
        //public bool GuildSubscriptions;
        //[JsonProperty(propertyName: "shard", Order = 5)]
        //public Array Shard;
        //[JsonProperty(propertyName: "presence", Order = 6)]
        //public Presences Presences;
        [JsonProperty(propertyName: "properties", Order = 1)]
        public IdentifyProperties Properties;
        [JsonProperty(propertyName: "intents", Order = 555)] //TODO : order, они вообще нужны?
        public string Intents;
        public Identify(string token,
                        IdentifyProperties properties,
                        IdentifyIntents intents = null,
                        bool compress = false,
                        int largeThreshold = 50,
                        bool guildSubscriptions = true)
        {
            this.Properties = properties;
            this.Token = token;
            this.Intents = intents is null ? null : intents.Intents;
            //this.Compress = compress;
            //this.LargeThreshold = largeThreshold;
            //this.GuildSubscriptions = guildSubscriptions;
        }
        //public Identify(string token,
        //                Presences presences,
        //                IdentityProperties properties,
        //                bool compress = false, int largeThreshold = 50,
        //                bool guildSubscriptions = true)
        //    : this(token, properties, compress, largeThreshold, guildSubscriptions)
        //{
        //    this.Presences = presences;
        //}
    }
    [JsonObject(MemberSerialization.OptIn)]
    internal class IdentifyProperties
    {
        [JsonProperty(propertyName: "$os", Order = 0)]
        public string Os = Environment.OSVersion.ToString();
        [JsonProperty(propertyName: "$browser", Order = 1)]
        public string Browser;
        [JsonProperty(propertyName: "$device", Order = 2)]
        public string Device;
        public IdentifyProperties(string browser, string device)
        {
            this.Browser = browser;
            this.Device = device;
        }
    }
    internal class IdentifyIntents
    {
        internal string Intents { get; private set; }
        internal static IdentifyIntents None = null,
                                        Default = new IdentifyIntents(1 << 0 | 1 << 2 | 1 << 3 |
                                                                      1 << 4 | 1 << 5 | 1 << 6 |
                                                                      1 << 7 | 1 << 9 | 1 << 10 |
                                                                      1 << 11 | 1 << 12 | 1 << 13 |
                                                                      1 << 14),
                                        Custom = new IdentifyIntents(GetCustomValue());
        private IdentifyIntents(int intents) => Intents = intents.ToString();
        private static int GetCustomValue() //TODO :чтение из конфига очвдно
        {
            return 0; 
        }
    }
}
