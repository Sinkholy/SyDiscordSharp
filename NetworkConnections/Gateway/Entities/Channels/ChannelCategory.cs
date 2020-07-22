using Gateway.Entities.Channels.Text;
using Gateway.Entities.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gateway.Entities.Channels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ChannelCategory : GuildTextChannelBase
    {
        #region Ctor's
        internal ChannelCategory(string id,
                                  ChannelType type,
                                  string lastMsgId,
                                  string guildId,
                                  string name,
                                  int position,
                                  List<Overwrite> permissionsOverwrite,
                                  bool nsfw,
                                  string parentId)
            : base(id, type, lastMsgId, guildId, name, position, permissionsOverwrite, nsfw, parentId)
        {
        }
        internal ChannelCategory(ChannelType type)
            : base(type) { }
        #endregion
    }
}