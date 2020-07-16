namespace Gateway.Payload.DataObjects.Enums
{
    public enum Events : short
    {
        UnknownEvent,
        Hello,
        Ready,
        Resumed,
        Reconnect,
        ChannelCreated,
        ChannelUpdated,
        ChannelDeleted,
        ChannelPinsUpdated,
        GuildCreated,
        GuildUpdated,
        GuildDeleted,
        GuildBanAdded,
        GuildBanDeleted,
        GuildEmojisUpdated,
        GuildIntegrationUpdated,
        GuildMemberAdded,
        GuildMemberRemoved,
        GuildMemberUpdated,
        GuildMembersChunkReceived,
        GuildRoleCreated,
        GuildRoleUpdated,
        GuildRoleDeleted,
        InviteCreated,
        InviteDeleted,
        MessageCreated,
        MessageUpdated,
        MessageDeleted,
        MessageDeletedBulk,
        MessageReactionAdded,
        MessageReactionRemoved,
        MessageReactionRemovedAll,
        MessageReactionEmojiRemoved,
        PresenceUpdated,
        TypingStarted,
        UserUpdated,
        VoiceStateUpdated,
        VoiceServerUpdated,
        WebhooksUpdated


        // TODO : тут пока непонятно как орбабатывать отдельно DM от Guild сообщения
        ////DIRECT_MESSAGES (1 << 12)
        //CHANNEL_CREATE,
        //MESSAGE_CREATE,
        //MESSAGE_UPDATE,
        //MESSAGE_DELETE,
        //CHANNEL_PINS_UPDATE,

        ////DIRECT_MESSAGE_REACTIONS (1 << 13)
        //MESSAGE_REACTION_ADD,
        //MESSAGE_REACTION_REMOVE,
        //MESSAGE_REACTION_REMOVE_ALL,
        //MESSAGE_REACTION_REMOVE_EMOJI,

        ////DIRECT_MESSAGE_TYPING (1 << 14)
        //TYPING_START
    }
}
