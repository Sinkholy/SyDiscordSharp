using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gateway.Enums;
using Gateway.PayloadObjects;
using Newtonsoft.Json;

namespace Gateway.Payload.DataObjects
{
    class GuildRequestMembers
    {
        [JsonProperty(propertyName: "guildId", Order = 0)]
        public string GuildId;
        [JsonProperty(propertyName: "query", Order = 1)]
        public string Query;
        [JsonProperty(propertyName: "limit", Order = 2)]
        public int Limit;

        public GuildRequestMembers(string guildId, string query, int limit)
        {
            this.GuildId = guildId;
            this.Query = query;
            this.Limit = limit;
        }
    }
}
