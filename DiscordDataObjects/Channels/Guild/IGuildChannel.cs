using System.Collections.Generic;

namespace DiscordDataObjects.Channels.Guild
{
    public interface IGuildChannel : IChannel
    {
        string GuildIdentifier { get; }
        string CategoryIdentifier { get; }
        string Name { get; }
        int Position { get; }
        bool NSFW { get; }
        IReadOnlyCollection<PermissionOverwrite> PermissionOverwrites { get; }
    }
}
