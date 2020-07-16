using Gateway.DataObjects.Channels;
using Gateway.DataObjects.Guilds;
using Gateway.DataObjects.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Voice
{
    internal class VoiceState
    {
        internal IUser User { get; private set; }
        [JsonProperty(PropertyName = "user_id")]
        internal string UserIdentifier { get; private set; }
        internal IChannel Channel { get; private set; }
        [JsonProperty(PropertyName = "channel_id")]
        internal string ChannelIdentifier { get; private set; }
        internal IGuild Guild { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "session_id")]
        internal string SessionIdentifier { get; private set; }
        [JsonProperty(PropertyName = "deaf")]
        internal bool Deafened { get; private set; }
        [JsonProperty(PropertyName = "mute")]
        internal bool Muted { get; private set; }
        [JsonProperty(PropertyName = "self_deaf")]
        internal bool SelfDeafened { get; private set; }
        [JsonProperty(PropertyName = "self_mute")]
        internal bool SelfMuted { get; private set; }
        [JsonProperty(PropertyName = "self_stream")]
        internal bool SelfStream { get; private set; }
        [JsonProperty(PropertyName = "suppress")]
        internal bool Suppressed { get; private set; }

        [JsonProperty(PropertyName = "member")]
        private GuildUser guildUser;
        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            IGuild targetGuild = null;
            if (GuildIdentifier != null)
                targetGuild = DiscordGatewayClient.TryToGetGuild(GuildIdentifier);
            IUser targetUser = null;
            IChannel targetChannel = null;
            if(targetGuild is Guild guild)
            {
                targetUser = guild.TryToGetUser(UserIdentifier);
                targetChannel = guild.TryToGetChannel(ChannelIdentifier);
            }
            if (targetUser == null && guildUser != null)
                targetUser = guildUser as IUser;
            Guild = targetGuild;
            User = targetUser;
            Channel = targetChannel;
        }
    }
}
