using Gateway.PayloadCommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.PayloadObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    class Identity : IGatewayDataObject
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
        public IdentityProperties Properties;

        public Identity(string token,
                        IdentityProperties properties,
                        bool compress = false,
                        int largeThreshold = 50,
                        bool guildSubscriptions = true)
        {
            this.Properties = properties;
            this.Token = token;
            //this.Compress = compress;
            //this.LargeThreshold = largeThreshold;
            //this.GuildSubscriptions = guildSubscriptions;
        }
        //public Identity(string token,
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
    public class IdentityProperties
    {
        [JsonProperty(propertyName: "$os", Order = 0)]
        public string Os = Environment.OSVersion.ToString();
        [JsonProperty(propertyName: "$browser", Order = 1)]
        public string Browser;
        [JsonProperty(propertyName: "$device", Order = 2)]
        public string Device;
        public IdentityProperties(string browser, string device)
        {
            this.Browser = browser;
            this.Device = device;
        }
    }
}
