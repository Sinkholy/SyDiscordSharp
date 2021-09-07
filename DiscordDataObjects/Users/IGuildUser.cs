using Gateway.Entities.Presences;
using System;
using System.Collections.Generic;

namespace DiscordDataObjects.Users
{ 
    public interface IGuildUser : IUser// TODO: поменять некоторые IUser на IGuildUser(по своему усмотрению)
    {
        IUser User { get; }
        string GuildIdentifier { get; }
        IReadOnlyCollection<string> RolesIdentifiers { get; }
        DateTime JoinedAt { get; }
        DateTime? PremiumSince { get; }
        string Nickname { get; }
        bool SelfDeafened { get; }
        bool SelfMuted { get; }
        bool SelfVideo { get; }
        bool SelfStream { get; }
        bool Deafeaned { get; }
        bool Muted { get; }
        IPresence Presence { get; }
    }
}
