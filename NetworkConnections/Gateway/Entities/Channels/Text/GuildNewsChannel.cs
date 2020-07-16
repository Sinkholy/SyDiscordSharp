using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GuildNewsChannel : GuildTextChannelBase
    {
        [JsonProperty(PropertyName = "topic")]
        internal string Topic;

        #region Ctor's
        internal GuildNewsChannel(string id,
                                  ChannelType type,
                                  string lastMsgId,
                                  string guildId,
                                  string name,
                                  int position,
                                  Overwrite[] permissionsOverwrite,
                                  bool nsfw,
                                  string parentId,
                                  string topic)
            : base(id, type, lastMsgId, guildId, name, position, permissionsOverwrite, nsfw, parentId)
        {
            Topic = topic;
        }
        internal GuildNewsChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}