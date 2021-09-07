using System.Collections.Generic;

using DiscordDataObjects.Channels.Guild.IUpdatable;

namespace DiscordDataObjects.Channels.Guild
{
    internal class GuildCategory : GuildChannel, IGuildCategory, IUpdatableGuildCategory
    {
        public new string CategoryIdentifier => string.Empty;
        #region Ctor's
        internal GuildCategory(string guildId,
                             string name,
                             bool nsfw,
                             int position,
                             List<PermissionOverwrite> permissionOverwrites)
            : base(ChannelType.GuildCategory, guildId, name, nsfw, position, permissionOverwrites, null)
        {
        }
        internal GuildCategory()
            : base(ChannelType.GuildCategory) { }
        #endregion
    }
}
