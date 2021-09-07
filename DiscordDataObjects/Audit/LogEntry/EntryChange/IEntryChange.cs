using System;

namespace DiscordDataObjects.Audit.LogEntry.EntryChange
{
    public interface IEntryChange
    {
        LogEntryChangeKey ChangeType { get; }
        object OldValueUntyped { get; }
        object NewValueUntyped { get; }
        EntryDataType DataType { get; }
    }
    public enum EntryDataType : byte
    {
        Integer,
        String,
        IRoleArray,
        IPermissionsOverwriteArray,
        Bool
    }
    public enum LogEntryChangeKey : short
    {
        Name,
        Icon,
        Splash,
        Owner,
        Region,
        AfkChannel,
        AfkTimeout,
        MFALevel,
        VerificationLevel,
        ExplicitContentFilter,
        DefaultMessageNotifications,
        VanityUrlCode,
        Added,
        Removed,
        PruneDaysCount,
        WidgetState,
        WidgetChannel,
        SystemChannel,
        Position,
        Topic,
        Bitrate,
        PermissionsOverwrite,
        NsfwState,
        ApplicationId,
        RateLimitPerUser,
        PermissionsLegacy,
        PermissionsNew,
        Color,
        HoistState,
        MentionableState,
        AllowLegacy,
        AllowNew,
        DenyLegacy,
        DenyNew,
        Code,
        ChannelId,
        InviterId,
        MaxUses,
        Uses,
        MaxAge,
        Temporary,
        Deaf,
        Mute,
        Nickname,
        Avatar,
        Id,
        Type,
        EmoticonsState,
        BehaviorExpire,
        GracePeriodExpire
    }
}
