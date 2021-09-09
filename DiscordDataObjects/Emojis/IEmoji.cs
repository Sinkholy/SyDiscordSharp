using DiscordDataObjects.Users;

namespace DiscordDataObjects.Emojis
{
    public interface IEmoji
    {
        string Identifier { get; }
        string Name { get; }
        string[] AvailableForRoles { get; }
        IUser CreatedBy { get; }
        bool? RequireColons { get; }
        bool? Managed { get; }
        bool? Animated { get; }
        bool? Available { get; }
        bool IsUnicodeEmoji { get; }
        string Mention { get; }
        string UrlEncoded { get; }
    }
}
