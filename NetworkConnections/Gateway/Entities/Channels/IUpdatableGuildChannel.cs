using System.Collections.Generic;

namespace Gateway.Entities.Channels
{
    public interface IUpdatableGuildChannel
    {
        void SetNewName(string name);
        void SetNewPosition(int position);
        void SetNewNsfw(bool nsfw);
        void SetNewPermissionsOverwrire(List<Overwrite> overwrite);
    }
}