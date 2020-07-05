using Gateway.DataObjects.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class DMTextChannel : TextChannel
    {
        [JsonProperty(PropertyName = "recipients")]
        internal IUser[] Recipients;

        #region Ctor's
        internal DMTextChannel(string id,
                               ChannelType type,
                               string lastMsgId,
                               IUser[] recipients)
            : base(id, type, lastMsgId)
        {
            Recipients = recipients;
        }
        internal DMTextChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}