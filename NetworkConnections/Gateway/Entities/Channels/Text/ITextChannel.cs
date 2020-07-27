using Gateway.Entities.Message;
using System.Collections.Generic;

namespace Gateway.Entities.Channels.Text
{
    public interface ITextChannel
    {
        void SendMessage(IMessage message);
        void SendMessage(string message);
        void RemoveMessage(string id);
        IReadOnlyCollection<IMessage> Messages { get; }
        IMessage TryToGetMessage(string id);
    }
}
