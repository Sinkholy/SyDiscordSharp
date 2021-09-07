using DiscordDataObjects.Channels;
using DiscordDataObjects.Guilds;
using DiscordDataObjects.Users;

namespace DiscordDataObjects.Webhook
{
    public interface IWebhook
    {
        string Identifier { get; }
        WebhookType Type { get; }
        string GuildIdentifier { get; }
        string ChannelIdentifier { get; }
        IUser CreatedBy { get; }
        string Name { get; }
        string Avatar { get; }
        string Token { get; }
        string ApplicationIdentifier { get; }
        IGuild SourceGuild { get; }
        IChannel SourceChannel { get; }
        string Url { get; }
    }
    public enum WebhookType : byte
    {
        Incoming = 1,
        ChannelFollower = 2,
        Application = 3
    }
}
