using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Text
{
    public interface ITextChannel
    {
        void SendMessage(IMessage message);
        void SendMessage(string message);
    }
}
