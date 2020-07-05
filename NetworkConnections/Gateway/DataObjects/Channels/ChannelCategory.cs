using Gateway.DataObjects.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gateway.DataObjects.Channels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ChannelCategory : Channel, IGuildChannel
    {
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }

        public void UpdateChannelGuildId(IGuild guild) => GuildIdentifier = guild.Identifier;

        #region Ctor's
        internal ChannelCategory(string id, ChannelType type)
            : base(id, type) { }
        internal ChannelCategory(ChannelType type)
            : base(type) { }
        #endregion
    }
}