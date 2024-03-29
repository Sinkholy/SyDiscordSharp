﻿using DiscordDataObjects.Emojis;

namespace DiscordDataObjects.Channels.Message
{
    internal interface IUpdatableMessage
    {
        void AddReaction(IEmoji emoji);
        void RemoveReaction(IEmoji emoji);
        void RemoveAllReactions();
        void RemoveAllEmojiReactions(IEmoji emoji);
    }
}