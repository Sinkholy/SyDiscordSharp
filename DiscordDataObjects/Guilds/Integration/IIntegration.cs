using Gateway.Entities.Users;
using System;

namespace DiscordDataObjects.Guilds.Integration
{
    public interface IIntegration
    {
        string Identifier { get; }
        string Name { get; }
        string Type { get; }
        bool Enabled { get; }
        bool Syncing { get; }
        string RoleIdentifier { get; }
        bool? IsEmoticonsEnabled { get; }
        IntegrationExpireBehavior ExpireBehavior { get; }
        int ExpireGracePeriod { get; }
        DateTime SyncedAt { get; }
        IUser User { get; }
        IIntegrationAccount Account { get; }
    }
}
