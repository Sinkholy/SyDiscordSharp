using Gateway.Entities.Presences;
using System;
using System.Collections.Generic;

namespace Gateway.Entities.Users
{ 
    public interface IGuildUser : IUser// TODO: поменять некоторые IUser на IGuildUser(по своему усмотрению)
    {
        string GuildIdentifier { get; }
        IReadOnlyCollection<Role> Roles { get; }
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
