﻿namespace Gateway.Payload.DataObjects.Enums
{
    public enum CloseCode
    {
        UnknownError = 4000,
        UnknownOpcode,
        DecodeError,
        NotAuthenticated,
        AuthenticationFailed,
        AlreadyAuthenticated,
        InvalidSeq = 4007,
        RateLimited,
        SessionTimedOut,
        InvalidShard,
        ShardingRequired,
        InvalidAPIVersion,
        InvalidIntents,
        DisallowedIntents
    }
}
