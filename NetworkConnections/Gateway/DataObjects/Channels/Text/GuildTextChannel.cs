using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GuildTextChannel : GuildTextChannelBase
    {
        [JsonProperty(PropertyName = "rate_limit_per_user")]
        internal int RateLimitPerUser;
        [JsonProperty(PropertyName = "topic")]
        internal string Topic;

        #region Ctor's
        internal GuildTextChannel(string id,
                                  ChannelType type,
                                  string lastMsgId,
                                  string guildId,
                                  string name,
                                  int position,
                                  Overwrite[] permissionsOverwrite,
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