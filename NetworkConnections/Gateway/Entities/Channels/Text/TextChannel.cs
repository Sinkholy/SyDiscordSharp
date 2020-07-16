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
