using DiscordDataObjects.Guilds.Presences;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class GuildMemberChunk
    {
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; } 
        [JsonProperty(PropertyName = "members")]
        internal GuildUser Users { get; private set; }
        [JsonProperty(PropertyName = "chunk_index")]
        internal int ChunkIndex { get; private set; }
        [JsonProperty(PropertyName = "chunk_count")]
        internal int ChunkCount { get; private set; }
        [JsonProperty(PropertyName = "not_found")]
        internal bool NotFound { get; private set; }
        [JsonProperty(PropertyName = "presences")]
        internal IPresence Presences { get; private set; }
        [JsonProperty(PropertyName = "nonce")]
        internal string Nonce { get; private set; }
    }
}