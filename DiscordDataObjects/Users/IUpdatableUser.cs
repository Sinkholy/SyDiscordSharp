using System.Collections.Generic;

using DiscordDataObjects.Guilds;
using DiscordDataObjects.Guilds.Presences;

namespace DiscordDataObjects.Users
{
    interface IUpdatableGuildUser
    {
        void UpdateGuildIdentifier(string guildId);
        void UpdateAvatar(string avatarHash);
        void UpdateUsername(string username);
        void UpdateSelfMutedState(bool selfMuted); // TAI: может все эти апдейты совместить?
        void UpdateSelfDeafenedState(bool selfDeafened);
        void UpdateMutedState(bool muted);
        void UpdateDeafenedState(bool deafened);
        void UpdateSelfStreamState(bool selfStream);
        void UpdateSelfVideoState(bool selfVideo);
        void UpdateRolesEnumerable(IEnumerable<Role> enumerable);
        void UpdatePresencesEnumerable(IEnumerable<IPresence> enumerable);
    }
}
