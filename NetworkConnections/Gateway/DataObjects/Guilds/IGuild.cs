using Gateway.DataObjects.Emojis;

namespace Gateway.DataObjects.Guilds
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
        void UpdateGuild(IGuild newGuildInfo);
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
