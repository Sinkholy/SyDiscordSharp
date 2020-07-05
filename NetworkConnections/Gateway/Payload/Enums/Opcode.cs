namespace Gateway.Enums
{
    public enum Opcode : byte
    {
        Dispatch = 0,
        Heartbeat,
        Identify,
        PresenceUpdate,
        VoiceStateUpdate,
        Resume = 6,
        Reconnect,
        RequestGuildMembers,
        InvalidSession,
        Hello,
        HeartbeatACK
    }
}
