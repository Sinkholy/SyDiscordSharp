using Gateway.Entities.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class DMTextChannel : TextChannel
    {
        [JsonProperty(PropertyName = "recipients")]
        internal IUser[] Recipients;
        public override string UpdateChannel(IChannel channelNewInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.UpdateChannel(channelNewInfo));
            DMTextChannel newChannel = channelNewInfo as DMTextChannel;
            if (newChannel is null)
            {
                DiscordGatewayClient.RaiseLog("Handling channel updated event. Cannot cast to DMTextChannel");
                return "";
            }
            else
            {
                //TODO : Recipients
            }
            return result.ToString();
        }
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