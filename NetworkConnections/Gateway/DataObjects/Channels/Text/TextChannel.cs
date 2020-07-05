using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class TextChannel : Channel, ITextChannel
    {
        [JsonProperty(PropertyName = "last_message_id")]
        internal string LastMessageIdentifier;

        #region ITextChannel implementation
        public void SendMessage(IMessage message) { }
        public void SendMessage(string message) { }
        #endregion
        #region Ctor's
        internal TextChannel(string id, ChannelType type, string lastMsgId)
            : base(id, type)
        {
            LastMessageIdentifier = lastMsgId;
        }
        internal TextChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}
