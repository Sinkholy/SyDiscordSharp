using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Text
{
    internal abstract class TextChannel : Channel, ITextChannel
    {
        internal IMessage LastMessage; //TODO : получение сообщения + при наличии обернуть в IMessage
        internal IMessage[] PinnedMessages; //А ещё лучше обернуть их в гетеры и сетеры через метод для сокрытия

        [JsonProperty(PropertyName = "last_message_id")]
        private string lastMessageIdentifier;

        #region IUpdatableChannel impl
        public override string UpdateChannel(IChannel newChannelInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.UpdateChannel(newChannelInfo));
            TextChannel newChannel = newChannelInfo as TextChannel;
            if(newChannel is null)
            {
                DiscordGatewayClient.RaiseLog("Handling channel updated event. Cannot cast to TextChannel");
                return "";
            }
            return result.ToString();
        }
        #endregion
        #region ITextChannel implementation
        public void SendMessage(IMessage message) { }
        public void SendMessage(string message) { }
        #endregion
        #region Ctor's
        internal TextChannel(string id, ChannelType type, string lastMsgId)
            : base(id, type)
        {
            lastMessageIdentifier = lastMsgId;
        }
        internal TextChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}
