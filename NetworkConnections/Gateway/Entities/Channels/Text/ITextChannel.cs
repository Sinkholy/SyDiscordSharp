using Gateway.Entities.Message;
using System.Collections.Generic;

namespace Gateway.Entities.Channels.Text
{
    public interface ITextChannel : IChannel
    {
        IReadOnlyCollection<IMessage> Messages { get; }
        IMessage TryToGetMessage(string id);
    }
}
