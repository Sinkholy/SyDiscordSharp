using Gateway.Entities.Users;

namespace Gateway.Entities.Webhook
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
    }
    public enum WebhookType : byte
    {
        Incoming,
        ChannelFollower
    }
}
