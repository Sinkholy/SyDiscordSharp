using Gateway.Entities.Emojis;
using Gateway.Entities.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class GuildEmojiUpdatedEvent
    {
        internal IGuild Guild { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "emoji")]
        internal Emoji Emoji { get; private set; }
        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild = DiscordGatewayClient.TryToGetGuild(GuildIdentifier);
        }
    }
}
