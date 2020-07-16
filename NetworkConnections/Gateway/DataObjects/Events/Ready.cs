using Gateway.DataObjects.Guilds;
using Gateway.DataObjects.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Events
{
    internal class Ready // TODO : Order's 
    {
        [JsonProperty(PropertyName = "v")]
        internal int GatewayVersion { get; private set; }
        [JsonProperty(PropertyName = "user")]
        internal User User { get; private set; }
        [JsonProperty(PropertyName = "guilds")]
        internal GuildPreview[] Guilds { get; private set; }
        [JsonProperty(PropertyName = "session_id")]
        internal string SessionIdentifier { get; private set; }
        //[JsonProperty(PropertyName = "application", Order = 4)]
        //internal ReadyApplication Application;
        [JsonProperty(PropertyName = "_trace")]
        internal object Trace { get; private set; }
        internal Ready() { }


        // Сейчас Application в gateway-документации никак не описан - заделка на будущее. 
        // Хотя при этом в json-объекте ready она присутствует 

        //[JsonObject(MemberSerialization.OptIn)]
        //internal class ReadyApplication
        //{
        //    [JsonProperty(PropertyName = "id", Order = 0)]
        //    internal string Identifier;
        //    [JsonProperty(PropertyName = "flags", Order = 1)]
        //    internal int Flags;
        //    internal ReadyApplication() { }
        //}
    }
}