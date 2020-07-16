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

namespace Gateway.DataObjects.Events
{
    internal class UserTypingEvent
    {
        internal IChannel Channel { get; private set; }
        internal IGuild Guild { get; private set; }
        internal IUser User { get; private set; }
        internal DateTime UtcTime { get; private set; }
        internal bool InGuild { get; private set; } = false;

        [JsonProperty(PropertyName = "timestamp")]
        private int timestamp;
        [JsonProperty(PropertyName = "guild_id")]
        private string guildIdentifier;
        [JsonProperty(PropertyName = "channel_id")]
        private string channelIdentifier;
        [JsonProperty(PropertyName = "user_id")]
        private string userIdentifier;
        [JsonProperty(PropertyName = "member")]
        private GuildUser user;
        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild targetGuild = DiscordGatewayClient.TryToGetGuild(guildIdentifier) as Guild;
            IChannel targetChannel = null;
            if (targetGuild != null)
            {
                targetChannel = targetGuild.TryToGetChannel(channelIdentifier);
            }
            IUser targetUser;
            if (user != null && targetGuild != null)
            {
                targetUser = targetGuild.TryToGetUser(userIdentifier);
                InGuild = true;
            }
            else targetUser = new User(userIdentifier);
            User = targetUser;
            Channel = targetChannel;
            Guild = targetGuild;
            UtcTime = ConvertDateTimeFromUnix(timestamp);
        }
        private DateTime ConvertDateTimeFromUnix(int seconds)
        {
            DateTime original = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return original.AddSeconds(seconds);
        }
    }
}
