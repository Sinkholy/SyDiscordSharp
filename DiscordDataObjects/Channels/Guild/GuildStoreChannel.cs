using System.Collections.Generic;

using DiscordDataObjects.Channels.Guild.IUpdatable;

namespace DiscordDataObjects.Channels.Guild
{
    internal class GuildStoreChannel : GuildChannel, IGuildStoreChannel, IUpdatableGuildStoreChannel
    {
        #region Ctor's
        internal GuildStoreChannel(string guildId,
                                 string name,
                                 bool nsfw,
                                 int position,
                                 List<PermissionOverwrite> permissionOverwrites,
                                 string categoryId = null)
            : base(ChannelType.GuildStore, guildId, name, nsfw, position, permissionOverwrites, categoryId)
        {
        }
        internal GuildStoreChannel()
            : base(ChannelType.GuildStore) { }
        #endregion
    }
}
