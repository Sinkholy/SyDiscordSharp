using Gateway.Entities.Message;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gateway.Entities.Channels.Text
{
    internal abstract class TextChannel : Channel, ITextChannel, IMessageEditableChannel
    {
        internal IMessage LastMessage => TryToGetMessage(lastMessageIdentifier);
        internal IMessage[] PinnedMessages; 

        [JsonProperty(PropertyName = "last_message_id")]
        private string lastMessageIdentifier; //TODO : потокобезопасность
        private List<IMessage> messages = new List<IMessage>(); //TODO : потокобезопасность

        #region IMessageEditableChannel
        void IMessageEditableChannel.AddMessage(IMessage message)
        {
            messages.Add(message);
            lastMessageIdentifier = message.Identifier;
        }
        #endregion
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
        public IReadOnlyCollection<IMessage> Messages => messages as IReadOnlyCollection<IMessage>;
        public void RemoveMessage(string id)
        {
            IMessage messageToDelete = TryToGetMessage(id);
            if (messageToDelete != null)
            {
                messages.Remove(messageToDelete);
            }
            if (messageToDelete.Identifier == lastMessageIdentifier)
            {
                lastMessageIdentifier = messages.FirstOrDefault().Identifier; //TODO : проверить сортирован ли список
            }
        }
        public IMessage TryToGetMessage(string id)
        {
            return messages.Where(x => x.Identifier == id).SingleOrDefault();
        }
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
