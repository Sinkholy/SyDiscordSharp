using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Guild.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GuildNewsChannel : GuildTextChannelBase
    {
        [JsonProperty(PropertyName = "topic")]
        internal string Topic;

        public override string UpdateChannel(IChannel newChannelInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.UpdateChannel(newChannelInfo));
            GuildNewsChannel newChannel = newChannelInfo as GuildNewsChannel;
            if(newChannel is null)
            {
                // TODO : инструмент логирования ("Handling channel updated event. Cannot cast to GuildNewsChannel");
                return "";
            }
            else
            {
                if(Topic != newChannel.Topic)
                {
                    Topic = newChannel.Topic;
                    result.Append("Topic |");
                }
            }
            return result.ToString();
        }
        #region Ctor's
        internal GuildNewsChannel(string id,
                                  ChannelType type,
                                  string lastMsgId,
                                  string guildId,
                                  string name,
                                  int position,
                                  List<Overwrite> permissionsOverwrite,
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