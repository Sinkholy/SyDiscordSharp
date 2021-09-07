using DiscordDataObjects.Channels;
using DiscordDataObjects.Guilds.Invite;
using DiscordDataObjects.Guilds.Presences;
using DiscordDataObjects.Users;
using DiscordDataObjects.VoiceSession;

namespace DiscordDataObjects.Guilds
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
