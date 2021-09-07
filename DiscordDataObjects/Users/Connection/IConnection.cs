using DiscordDataObjects.Guilds.Integration;

using System.Collections.Generic;

namespace DiscordDataObjects.Users.Connection
{
    public interface IConnection
    {
        string Identifier { get; }
        string Name { get; }
        string Type { get; }
        bool? IsRevoked { get; }
        IReadOnlyCollection<IIntegration> Integrations { get; }
        bool IsVerified { get; }
        bool IsFriendSync { get; }
        bool IsShowActivity { get; }
        VisibilityType Visibility { get; }
    }
    public enum VisibilityType : byte
    {
        None,
        Everyone
    }
}
