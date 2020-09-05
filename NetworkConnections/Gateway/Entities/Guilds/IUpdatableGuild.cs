using Gateway.Entities.Channels;
using Gateway.Entities.Emojis;
using Gateway.Entities.Invite;
using Gateway.Entities.Presences;
using Gateway.Entities.Users;
using Gateway.Entities.VoiceSession;

namespace Gateway.Entities.Guilds
{
    internal interface IUpdatableGuild
    {
        Guild LoadInfoFromOldGuild(Guild newGuildInfo);
        void AddChannel(IChannel channel);
        void RemoveChannel(string channelId);
        IChannel OverrideChannel(IChannel newChannelInfo);
        void AddRole(Role role);
        void RemoveRole(string roleId);
        Role OverrideRole(Role newRoleInfo);
        void AddInvite(IInvite invite);
        void RemoveInvite(string inviteId);
        void AddBan(Ban ban);
        void RemoveBan(string userId);
        void AddUser(GuildUser user);
        void RemoveUser(string userId);
        GuildUser OverrideGuildUser(GuildUser newUser);
        void AddVoiceSession(IVoiceSession session);
        IVoiceSession RemoveVoiceSession(string sessionId);
        IVoiceSession OverrideVoiceSession(IVoiceSession newStateInfo);
        void AddPresence(IPresence newPresence);
        void RemovePresence(string userId);
        IPresence OverridePresence(IPresence newPresenceInfo);
        GuildEmoji[] SetNewGuildEmojis(GuildEmoji[] emojis);
    }
}
