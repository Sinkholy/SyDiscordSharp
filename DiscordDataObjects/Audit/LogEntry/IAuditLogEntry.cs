﻿using System.Collections.Generic;

using DiscordDataObjects.Audit.LogEntry.EntryChange;
using DiscordDataObjects.Audit.LogEntry.EntryOptionalInfo;

namespace DiscordDataObjects.Audit.LogEntry
{
    public interface IAuditLogEntry
    {
        string TargetIdentifier { get; }
        string UserIdentifier { get; }
        string Identifier { get; }
        AuditLogEntryType EntryType { get; }
        string Reason { get; }
        IReadOnlyCollection<IEntryChange> Changes { get; }
        IOptionalEntryInfo OptionalInfo { get; }
    }
    public enum AuditLogEntryType : byte
    {
        GUILD_UPDATE = 1,
        CHANNEL_CREATE = 10,
        CHANNEL_UPDATE = 11,
        CHANNEL_DELETE = 12,
        CHANNEL_OVERWRITE_CREATE = 13,
        CHANNEL_OVERWRITE_UPDATE = 14,
        CHANNEL_OVERWRITE_DELETE = 15,
        MEMBER_KICK = 20,
        MEMBER_PRUNE = 21,
        MEMBER_BAN_ADD = 22,
        MEMBER_BAN_REMOVE = 23,
        MEMBER_UPDATE = 24,
        MEMBER_ROLE_UPDATE = 25,
        MEMBER_MOVE = 26,
        MEMBER_DISCONNECT = 27,
        BOT_ADD = 28,
        ROLE_CREATE = 30,
        ROLE_UPDATE = 31,
        ROLE_DELETE = 32,
        INVITE_CREATE = 40,
        INVITE_UPDATE = 41,
        INVITE_DELETE = 42,
        WEBHOOK_CREATE = 50,
        WEBHOOK_UPDATE = 51,
        WEBHOOK_DELETE = 52,
        EMOJI_CREATE = 60,
        EMOJI_UPDATE = 61,
        EMOJI_DELETE = 62,
        MESSAGE_DELETE = 72,
        MESSAGE_BULK_DELETE = 73,
        MESSAGE_PIN = 74,
        MESSAGE_UNPIN = 75,
        INTEGRATION_CREATE = 80,
        INTEGRATION_UPDATE = 81,
        INTEGRATION_DELETE = 82
    }
}
