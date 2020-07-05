using Gateway.DataObjects.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class GuildTextChannelBase : TextChannel, IGuildChannel
    {
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        internal string Name;
        [JsonProperty(PropertyName = "position")]
        internal int Position;
        [JsonProperty(PropertyName = "permission_overwrites")]
        internal Overwrite[] PermissionsOverwrite;
        [JsonProperty(PropertyName = "nsfw")]
        internal bool NSFW;
        [JsonProperty(PropertyName = "parent_id")]
        internal string ParentIdentifier;

        public void UpdateChannelGuildId(IGuild guild) => GuildIdentifier = guild.Identifier;

        #region Ctor's
        internal GuildTextChannelBase(string id,
                                      ChannelType type,
                                      string lastMessageId,
                                      string guildId,
                                      string name,
                                      int position,
                                      Overwrite[] permissionsOverwrite,
                                      bool nsfw,
                                      string parentId)
            : base(id, type, lastMessageId)
        {
            GuildIdentifier = guildId;
            Name = name;
            PermissionsOverwrite = permissionsOverwrite;
            NSFW = nsfw;
            ParentIdentifier = parentId;
            Position = position;
        }
        internal GuildTextChannelBase(ChannelType type)
            : base(type) { }
        #endregion
    }
}
