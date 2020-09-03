using Gateway.Entities.Emojis;
using Gateway.Entities.VoiceSession;
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
        GuildEmoji[] Emojis { get; }
        bool Unavailable { get; }
        GuildFeatures[] Features { get; }
        int ApproximatePresenceCount { get; }
        int ApproximateMemberCount { get; }
        IReadOnlyCollection<IVoiceSession> ActiveVoiceSessions { get; }

        IVoiceSession TryToGetVoiceSession(string sessionId);
    }
    public enum GuildFeatures
    {
        InviteSplash,
        VipRegions,
        VanityUrl,
        Verified,
        Partnered,
        Public,
        Commerce,
        News,
        Discoverable,
        Featurable,
        AnimatedIcon,
        Banner,
        PublicDisabled,
        WelcomeScreenEnabled
    }
}
