using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class VoiceServerUpdate
    {
        [JsonProperty(PropertyName = "token")]
        internal string Token { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "endpoint")]
        internal string Endpoint { get; private set; }
    }
}
