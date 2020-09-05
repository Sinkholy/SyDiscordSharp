using Gateway.Entities.Message;

namespace Gateway.Entities.Channels.Text
{
    internal interface IUpdatableTextChannel
    {
        void AddMessage(IMessage message);
        void RemoveMessage(string messageId);
    }
}
