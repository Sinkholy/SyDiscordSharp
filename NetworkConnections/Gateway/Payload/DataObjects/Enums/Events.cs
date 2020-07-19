namespace Gateway.Payload.DataObjects.Enums
{
    public enum Events : short
    {
        UnknownEvent,
        HELLO,
        READY,
        RESUMED,
        RECONNECT,
        CHANNEL_CREATE,
        CHANNEL_UPDATE,
        CHANNEL_DELETE,
        CHANNEL_PINS_UPDATE,
        GUILD_CREATE,
        GUILD_UPDATE,
        GUILD_DELETE,
        GUILD_BAN_ADD,
        GUILD_BAN_REMOVE,
        GUILD_EMOJIS_UPDATE,
        GUILD_INTEGRATIONS_UPDATE,
        GUILD_MEMBER_ADD,
        GUILD_MEMBER_REMOVE,
        GUILD_MEMBER_UPDATE,
        GuildMembersChunkReceived,
        GUILD_ROLE_CREATE,
        GUILD_ROLE_UPDATE,
        GUILD_ROLE_DELETE,
        INVITE_CREATE,
        INVITE_DELETE,
        MESSAGE_CREATE,
        MESSAGE_UPDATE,
        MESSAGE_DELETE,
        MESSAGE_DELETE_BULK,
        MESSAGE_REACTION_ADD,
        MESSAGE_REACTION_REMOVE,
        MESSAGE_REACTION_REMOVE_ALL,
        MESSAGE_REACTION_REMOVE_EMOJI,
        PRESENCE_UPDATE,
        TYPING_START,
        USER_UPDATE,
        VOICE_STATE_UPDATE,
        VOICE_SERVER_UPDATE,
        WEBHOOKS_UPDATE


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
