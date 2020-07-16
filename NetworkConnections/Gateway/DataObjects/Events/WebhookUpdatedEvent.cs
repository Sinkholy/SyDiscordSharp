using Gateway.DataObjects.Channels;
using Gateway.DataObjects.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Events
{
    internal class WebhookUpdatedEvent
    {
        internal IGuild Guild { get; private set; }
        internal IChannel Channel { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "channel_id")]
        internal string ChannelIdentifier { get; private set; }
        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild = DiscordGatewayClient.TryToGetGuild(GuildIdentifier);
            if (Guild is Guild guild)
                Channel = guild.TryToGetChannel(ChannelIdentifier);
        }
    }
}
