using System.Collections.Generic;

namespace Gateway.Entities.Channels.Guild.IUpdatable
{
    public interface IUpdatableGuildChannel : IUpdatableChannel
    {
        void SetNewName(string name);
        void SetNewPosition(int position);
        void SetNewNsfw(bool nsfw);
        void SetNewPermissionsOverwrire(List<PermissionOverwrite> overwrite);
        void SetNewCategory(string categoryId);
        void SetNewGuildId(string guildId);
    }
}
