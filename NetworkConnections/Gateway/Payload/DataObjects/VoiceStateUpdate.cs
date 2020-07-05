using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gateway.Enums;
using Newtonsoft.Json;

namespace Gateway.Payload.DataObjects
{
    class VoiceStateUpdate
    {
        [JsonProperty(propertyName: "guildId", Order = 0)]
        public string GuildId;
        [JsonProperty(propertyName: "channelId", Order = 0)]
        public string ChannelId;
        [JsonProperty(propertyName: "selfMute", Order = 0)]
        public bool SelfMute;
        [JsonProperty(propertyName: "selfDeaf", Order = 0)]
        public bool SelfDeaf;

        public VoiceStateUpdate(string guildId, string channelId, bool selfMute, bool selfDeaf)
        {
            this.GuildId = guildId;
            this.ChannelId = channelId;
            this.SelfMute = selfMute;
            this.SelfDeaf = selfDeaf;
        }
    }
}
