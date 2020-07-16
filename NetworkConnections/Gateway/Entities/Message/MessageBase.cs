using Gateway.Entities.Channels;
using Gateway.Entities.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Message
{
    internal class MessageBase : IMessage
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        public IGuild Guild { get; private protected set; }
        public IChannel Channel { get; private protected set; }

        [JsonProperty(PropertyName = "guild_id")]
        private protected string guildIdentifier;
        [JsonProperty(PropertyName = "channel_id")]
        private protected string channelIdentifier;

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild targetGuild = DiscordGatewayClient.TryToGetGuild(guildIdentifier) as Guild;
            IChannel targetChannel = null;
            if (targetGuild != null)
            {
                targetChannel = targetGuild.TryToGetChannel(channelIdentifier);
            }
            Guild = targetGuild;
            Channel = targetChannel;
        }
    }
}
