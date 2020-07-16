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