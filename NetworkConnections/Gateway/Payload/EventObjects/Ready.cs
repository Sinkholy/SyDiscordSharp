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

namespace Gateway.Payload.EventObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Ready : IEvent // TODO : Order's 
    {
        [JsonProperty(PropertyName = "v", Order = 0)]
        internal int GatewayVersion;
        [JsonProperty(PropertyName = "user", Order = 1)]
        internal User User;
        [JsonProperty(PropertyName = "guilds", Order = 2)]
        internal List<UnavailableGuild> Guilds;
        [JsonProperty(PropertyName = "session_id", Order = 3)]
        internal string SessionIdentifier;
        //[JsonProperty(PropertyName = "application", Order = 4)]
        //internal ReadyApplication Application;
        [JsonProperty(PropertyName = "_trace", Order = 5)]
        internal object Trace;
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