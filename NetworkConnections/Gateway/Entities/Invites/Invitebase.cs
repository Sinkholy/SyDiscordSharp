using Gateway.Entities.Channels;
using Gateway.Entities.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Invite
{
    internal class InviteBase : IInvite
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; private protected set; }
        public IGuild Guild { get; private protected set; }
        public IChannel Channel { get; private protected set; }

        [JsonProperty(PropertyName = "guild_id")]
        internal protected string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "channel_id")]
        internal protected string ChannelIdentifier { get; private set; }

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild targetGuild = DiscordGatewayClient.TryToGetGuild(GuildIdentifier) as Guild;
            IChannel targetChannel = null;            
            if (targetGuild != null)
                targetChannel = targetGuild.TryToGetChannel(ChannelIdentifier);
            Guild = targetGuild;
            Channel = targetChannel;
        }
    }
}
