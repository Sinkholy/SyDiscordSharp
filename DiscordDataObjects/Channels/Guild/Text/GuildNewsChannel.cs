﻿using System.Collections.Generic;

using DiscordDataObjects.Channels.Guild.IUpdatable;

namespace DiscordDataObjects.Channels.Guild.Text
{
    internal class GuildNewsChannel : GuildTextChannelBase, IGuildNewsChannel, IUpdatableGuildNewsChannel
    {
        #region Ctor's
        internal GuildNewsChannel(string guildId,
                                  string name,
                                  bool nsfw,
                                  int position,
                                  List<PermissionOverwrite> permissionOverwrites,
                                  int rateLimit,
                                  string topic = null,
                                  string lastMessageId = null,
                                  string categoryId = null)
            : base(ChannelType.GuildNews, guildId, name, nsfw, position, permissionOverwrites, rateLimit, topic, lastMessageId, categoryId)
        {
        }
        internal GuildNewsChannel()
            : base(ChannelType.GuildNews) { }
        #endregion
    }
}
