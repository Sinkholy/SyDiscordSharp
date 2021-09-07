using DiscordDataObjects.Guilds;
using DiscordDataObjects.Users;

using System;

namespace DiscordDataObjects.Emojis
{
    public interface IEmoji
    {
        string Identifier { get; }
        string Name { get; }
        Role[] AvailableForRoles { get; }
        IUser CreatedBy { get; }
        bool RequireColons { get; }
        bool Managed { get; }
        bool IsAnimated { get; }
        bool Available { get; }
        bool IsUnicodeEmoji { get; }
        string Mention { get; }
        string UrlEncoded { get; }
    }
}
