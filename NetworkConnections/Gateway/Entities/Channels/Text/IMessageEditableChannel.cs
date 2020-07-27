using Gateway.Entities.Message;

namespace Gateway.Entities.Channels.Text
{
    internal interface IMessageEditableChannel
    {
        void AddMessage(IMessage message);
    }
}
