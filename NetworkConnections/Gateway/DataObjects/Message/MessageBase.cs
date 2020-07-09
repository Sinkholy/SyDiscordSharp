using Gateway.DataObjects.Channels;
using Gateway.DataObjects.Guilds;
using Gateway.Payload.EventObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Message
{
    internal class MessageBase : IMessage, IEvent
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
            DiscordGatewayClient gatewayClient = DiscordGatewayClient.GetInstance();
            Guild targetGuild;
            IChannel targetChannel;

            if (gatewayClient.guilds[this.guildIdentifier] is Guild)
            {
                targetGuild = gatewayClient.guilds[this.guildIdentifier] as Guild;
                targetChannel = targetGuild.Channels.Where(x => x.Identifier == channelIdentifier).SingleOrDefault();
            }
            else
            {
                throw new Exception();//TODO : исключение или зачем?
            }
            Guild = targetGuild;
            Channel = targetChannel;
        }
    }
}
