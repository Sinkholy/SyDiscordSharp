using System.Collections.Generic;

namespace Gateway.Entities.Channels.Guild.Text
{
    internal class GuildTextChannel : GuildTextChannelBase
    {
        #region Ctor's
        internal GuildTextChannel(string guildId,
                                  string name,
                                  bool nsfw,
                                  int position,
                                  List<PermissionOverwrite> permissionOverwrites,
                                  int rateLimit,
                                  string topic = null,
                                  string lastMessageId = null,
                                  string categoryId = null)
            : base(ChannelType.GuildText, guildId, name, nsfw, position, permissionOverwrites, rateLimit, topic, lastMessageId, categoryId)
        {
        }
        internal GuildTextChannel()
            : base(ChannelType.GuildText) { }
        #endregion
    }
}
