using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GuildTextChannel : GuildTextChannelBase
    {
        [JsonProperty(PropertyName = "rate_limit_per_user")]
        internal int RateLimitPerUser;
        [JsonProperty(PropertyName = "topic")]
        internal string Topic;

        public override string UpdateChannel(IChannel newChannelInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.UpdateChannel(newChannelInfo));
            GuildTextChannel newChannel = newChannelInfo as GuildTextChannel;
            if (newChannel is null)
            {
                DiscordGatewayClient.RaiseLog("Handling channel updated event. Cannot cast to GuildTextChannel");
                return string.Empty;
            }
            else
            {
                if(RateLimitPerUser != newChannel.RateLimitPerUser)
                {
                    RateLimitPerUser = newChannel.RateLimitPerUser;
                    result.Append("RateLimitPeruser |");
                }
                if(Topic != newChannel.Topic)
                {
                    Topic = newChannel.Topic;
                    result.Append("Topic |");
                }
            }
            return result.ToString();
        }
        #region Ctor's
        internal GuildTextChannel(string id,
                                  ChannelType type,
                                  string lastMsgId,
                                  string guildId,
                                  string name,
                                  int position,
                                  List<Overwrite> permissionsOverwrite,
                                  bool nsfw,
                                  string parentId,
                                  int rateLimitPerUser,
                                  string topic)
            : base(id, type, lastMsgId, guildId, name, position, permissionsOverwrite, nsfw, parentId)
        {
            Topic = topic;
            RateLimitPerUser = rateLimitPerUser;
        }
        internal GuildTextChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}