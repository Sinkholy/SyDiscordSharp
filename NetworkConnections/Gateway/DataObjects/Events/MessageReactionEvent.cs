using Gateway.DataObjects.Channels;
using Gateway.DataObjects.Emojis;
using Gateway.DataObjects.Guilds;
using Gateway.DataObjects.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Events
{
    internal class MessageReactionEvent
    {
        internal IUser User { get; private set; }
        internal IGuild Guild { get; private set; }
        internal IChannel Channel { get; private set; }
        [JsonProperty(PropertyName = "message_id")]
        internal string MessageIdentifier { get; private set; } // TODO : класть сообщение, а не айди
        [JsonProperty(PropertyName = "emoji")]
        internal Emoji Emoji;

        [JsonProperty(PropertyName = "guild_id")]
        private string guildIdentifier;
        [JsonProperty(PropertyName = "channel_id")]
        private string channelIdentifier;
        [JsonProperty(PropertyName = "user_id")]
        private string userIdentifier;
        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild targetGuild = DiscordGatewayClient.TryToGetGuild(guildIdentifier) as Guild;
            IUser targetUser = null;
            IChannel targetChannel = null;
            if (targetGuild != null)
            {
                targetChannel = targetGuild.TryToGetChannel(channelIdentifier);
                targetUser = targetGuild.TryToGetUser(userIdentifier);
            }
            User = targetUser;
            Channel = targetChannel;
            Guild = targetGuild;
        }
    }
}
