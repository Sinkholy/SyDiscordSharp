using Gateway.DataObjects.Voice;
using Gateway.Entities.Channels;
using Gateway.Entities.Channels.Text;
using Gateway.Entities.Channels.Voice;
using Gateway.Entities.Emojis;
using Gateway.Entities.Invite;
using Gateway.Entities.Presences;
using Gateway.Entities.Users;
using Gateway.Entities.VoiceSession;
using Gateway.Payload.DataObjects.Dispatch.DispatchEvents;
using System;
using System.Collections.Generic;

namespace Gateway.Entities.Guilds
{
    public interface IGuild
    {
        string Identifier { get; }
        string Name { get; }
        string Icon { get; }
        string Splash { get; }
        string DiscoverySplash { get; }
        string Description { get; }
        bool Unavailable { get; }
        GuildFeatures[] Features { get; }
        int ApproximatePresenceCount { get; }
        int ApproximateMemberCount { get; }
        IUser Owner { get; }
        TimeSpan? AfkTimeout { get; }
        bool IsOwner { get; }
        int? Permissions { get; }
        MFA MultifactorAuth { get; } // TODO: bool?
        string ApplicationIdentifier { get; }
        bool WigdetEnabled { get; }
        SystemChannelFlags SysChnlFlags { get; } // TODO: нужно ли?
        VerificationLevel VerificationLevel { get; }
        DefaultMessageNotificationLevel DefaultMessageNotificationLevel { get; }
        ExplicitContentFilterLevel ExplicitContentFilterLevel { get; }
        DateTime JoinedAt { get; }
        bool Large { get; }
        int MemberCount { get; }
        int? MaxPresences { get; }
        string Region { get; }
        int MaxMembers { get; }
        string VanityUrlCode { get; }
        string Banner { get; }
        PremiumTier PremTier { get; }
        int PremiumSubscriptionsCount { get; }
        string PreferredLocale { get; }
        int MaxVideoChannelUsers { get; }
        #region RO collections
        IReadOnlyCollection<GuildEmoji> Emojis { get; }
        IReadOnlyCollection<IChannel> Channels { get; }
        IReadOnlyCollection<IChannelCategory> ChannelCategories { get; }
        IReadOnlyCollection<IVoiceChannel> VoiceChannels { get; }
        IReadOnlyCollection<ITextChannel> TextChannels { get; }
        IReadOnlyCollection<IInvite> Invites { get; }
        IReadOnlyCollection<IUser> Users { get; }
        IReadOnlyCollection<IUser> BannedUsers { get; }
        IReadOnlyCollection<Role> Roles { get; }
        IReadOnlyCollection<IVoiceSession> ActiveVoiceSessions { get; }
        IReadOnlyCollection<IPresence> Presences { get; }
        #endregion
        #region Channels
        IChannel PublicUpdatesChannel { get; }
        IChannel WidgetChannel { get; }
        ITextChannel RulesChannel { get; }
        ITextChannel SystemChannel { get; }
        IVoiceChannel AfkChannel { get; }
        #endregion
        #region TryToGet
        IChannel TryToGetChannel(string channelId);
        IUser TryToGetUser(string userId);
        Role TryToGetRole(string roleId);
        IInvite TryToGetInvite(string inviteCode);
        Ban TryToGetBannedUser(string userId);
        IVoiceSession TryToGetVoiceSession(string sessionId);
        GuildEmoji TryToGetEmoji(string id);
        IPresence TryToGetPresence(string userId);
        #endregion
    }
    public enum GuildFeatures
    {
        INVITE_SPLASH,
        VIP_REGIONS,
        VANITY_URL,
        VERIFIED,
        PARTNERED,
        PUBLIC,
        COMMERCE,
        NEWS,
        DISCOVERABLE,
        FEATURABLE,
        ANIMATED_ICON,
        BANNER,
        PUBLIC_DISABLED,
        WELCOME_SCREEN_ENABLED,
        COMMUNITY
    }
    [Flags]
    public enum SystemChannelFlags
    {
        SuppressJoinNotification = 1 << 0,
        SuppressPremiumSubscriptions = 1 << 1
    }
    public enum DefaultMessageNotificationLevel : byte { AllMessages, OnlyMentions }
    public enum MFA : byte { None, Elevated }
    public enum PremiumTier : byte
    {
        None,
        FirstTier,
        SecondTier,
        ThirdTier
    }
    public enum ExplicitContentFilterLevel : byte
    {
        Disabled,
        MembersWithoutRoles,
        AllMembers
    }
    public enum VerificationLevel : byte
    {
        None,
        Low,
        Medium,
        High,
        VeryHigh
    }
}
