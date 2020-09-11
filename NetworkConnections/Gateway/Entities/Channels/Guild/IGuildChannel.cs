using System.Collections.Generic;

namespace Gateway.Entities.Channels
{
    public interface IGuildChannel : IChannel
    {
        string GuildIdentifier { get; }
        string Name { get; }
        int Position { get; }
        bool NSFW { get; }
        IReadOnlyCollection<Overwrite> PermissionsOverwrite { get; }
        string CategoryIdentifier { get; }

        void UpdateChannelGuildId(string guildId);
    }
}
