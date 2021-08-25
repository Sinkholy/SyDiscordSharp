using Newtonsoft.Json;
using System;

namespace Gateway.Entities.Channels
{
    internal abstract class Channel : IChannel, IUpdatableChannel
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        public ChannelType Type { get; private set; }

        [JsonProperty(PropertyName = "type")]
        private int type;

        internal static ChannelType GetChannelType(string type)
            => (ChannelType)Enum.Parse(typeof(ChannelType), type);

        #region IUpdatableChannel implementation
        void IUpdatableChannel.UpdateChannelType(ChannelType type)
        {
            Type = type;
        }
        #endregion
        #region Ctor's
        internal Channel(ChannelType type)
        {
            Type = type;
            this.type = (int)type;
        }
        #endregion
    }
    public enum ChannelType : byte
    {
        GuildText,
        DirectMessage,
        GuildVoice,
        GroupDirectMessage,
        GuildCategory,
        GuildNews,
        GuildStore
    }
}
