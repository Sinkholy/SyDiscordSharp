using Gateway.Entities.Channels.Guild.IUpdatable;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordDataObjects.Channels.Guild.Text
{
    internal abstract class GuildTextChannelBase : GuildChannel, ITextChannel, IGuildTextChannel, IUpdatableGuildTextChannel
    {
        [JsonProperty(PropertyName = "last_message_id")]
        public string LastMessageIdentifier { get; private set; }
        [JsonProperty(PropertyName = "topic")]
        public string Topic { get; private set; }
        [JsonProperty(PropertyName = "rate_limit_per_user")]
        public int RateLimitPerUser { get; private set; }

        #region IUpdatableGuildTextChannel
        void IUpdatableGuildTextChannel.SetNewRateLimit(int rateLimit)
        {
            if(rateLimit < 0 || rateLimit > 21600)
            {
                throw new ArgumentOutOfRangeException("Text channel rate limit must be in range 0 - 21600 seconds.");
            }
            RateLimitPerUser = rateLimit;
        }
        void IUpdatableGuildTextChannel.SetNewTopic(string topic)
        {
            if(topic.Length > 1024)
            {
                throw new ArgumentOutOfRangeException("Topic length must be less then 1024 symbols.");
            }
            Topic = topic;
        }
        void IUpdatableGuildTextChannel.SetNewLastMessageIdentifier(string messageId)
        {
            LastMessageIdentifier = messageId;
        }
        #endregion
        #region Ctor's
        private protected GuildTextChannelBase(ChannelType type,
                                               string guildId,
                                               string name,
                                               bool nsfw,
                                               int position,
                                               List<PermissionOverwrite> permissionOverwrites,
                                               int rateLimit,
                                               string topic = null,
                                               string lastMessageId = null,
                                               string categoryId = null)
            : base(type, guildId, name, nsfw, position, permissionOverwrites, categoryId)
        {
            RateLimitPerUser = rateLimit;
            Topic = topic;
            LastMessageIdentifier = lastMessageId;
        }
        private protected GuildTextChannelBase(ChannelType type)
            : base(type) { }
        #endregion
    }
}
