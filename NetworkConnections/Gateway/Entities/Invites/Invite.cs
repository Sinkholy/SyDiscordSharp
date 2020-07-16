using Gateway.Entities.Channels;
using Gateway.Entities.Guilds;
using Gateway.Entities.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Invite
{
    internal class Invite : InviteBase
    {
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; private set; }
        /// <remarks>
        /// If inviter is in guild now then GuildUser will be passed, User otherwise.
        /// </remarks>
        public IUser Inviter { get; private set; }
        public TimeSpan ValidFor { get; private set; }
        [JsonProperty(PropertyName = "max_uses")]
        public int MaxUses { get; private set; }
        [JsonProperty(PropertyName = "temporary")]
        public bool Temporary { get; private set; }
        [JsonProperty(PropertyName = "uses")]
        public int Uses { get; private set; }
        public IUser TargetUser => targetUser as IUser;

        [JsonProperty(PropertyName = "target_user")]
        internal User targetUser { get; private set; }
        [JsonProperty(PropertyName = "inviter")]
        internal User inviter { get; private set; }

        [JsonProperty(PropertyName = "max_age")]
        private int validFor;

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild targetGuild = DiscordGatewayClient.TryToGetGuild(GuildIdentifier) as Guild;
            IChannel targetChannel = null;
            IUser targetInviter = null;
            if (targetGuild != null)
            {
                targetChannel = targetGuild.TryToGetChannel(ChannelIdentifier);
                targetInviter = targetGuild.TryToGetUser(inviter.Identifier);
            }
            ValidFor = validFor > 0 
                     ? TimeSpan.FromSeconds(validFor) 
                     : TimeSpan.Zero;
            Inviter = targetInviter;
            Guild = targetGuild;
            Channel = targetChannel;
        }
    }
}
