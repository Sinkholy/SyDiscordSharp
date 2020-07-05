using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GuildStoreChannel : GuildTextChannelBase
    {
        #region Ctor's
        internal GuildStoreChannel(string id,
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
        }
        internal GuildStoreChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}