using Gateway.Entities.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GroupDMTextChannel : DMTextChannel
    {
        [JsonProperty(PropertyName = "owner_id")]
        internal string OwnerIdentifier;
        [JsonProperty(PropertyName = "icon")]
        internal string IconHash;
        [JsonProperty(PropertyName = "name")]
        internal string Name;

        public override string UpdateChannel(IChannel channelNewInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.UpdateChannel(channelNewInfo));
            GroupDMTextChannel newChannel = channelNewInfo as GroupDMTextChannel;
            if (newChannel is null)
            {
                DiscordGatewayClient.RaiseLog("Handling channel updated event. Cannot cast to DMTextChannel");
                return "";
            }
            else
            {
                if(IconHash != newChannel.IconHash)
                {
                    IconHash = newChannel.IconHash;
                    result.Append("Icon |");
                }
                if(Name != newChannel.Name)
                {
                    Name = newChannel.Name;
                    result.Append("Name |");
                }
                if(OwnerIdentifier != newChannel.OwnerIdentifier)
                {
                    OwnerIdentifier = newChannel.OwnerIdentifier;
                    result.Append("Owner |");
                }
            }
            return result.ToString();
        }
        #region Ctor's
        internal GroupDMTextChannel(string id,
                                    ChannelType type,
                                    string lastMsgId,
                                    IUser[] recipients,
                                    string ownerId,
                                    string iconHash,
                                    string name)
            : base(id, type, lastMsgId, recipients)
        {
            OwnerIdentifier = ownerId;
            IconHash = iconHash;
            Name = name;
        }
        internal GroupDMTextChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}