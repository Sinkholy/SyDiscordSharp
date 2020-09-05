using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gateway.Entities.Channels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ChannelCategory : Channel, IGuildChannel, IChannelCategory, IUpdatableGuildCategory
    {
        public string CategoryIdentifier => string.Empty;
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "position")]
        public int Position { get; private set; }
        [JsonProperty(PropertyName = "nsfw")]
        public bool NSFW { get; private set; }
        public IReadOnlyCollection<Overwrite> PermissionsOverwrite => permissionsOverwrite;
        [JsonProperty(PropertyName = "permission_overwrites")]
        private List<Overwrite> permissionsOverwrite;
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        #region IUpdatableGuildCategory implementation
        void IUpdatableGuildChannel.SetNewName(string name)
        {
            Name = name;
        }
        void IUpdatableGuildChannel.SetNewNsfw(bool nsfw)
        {
            NSFW = nsfw;
        }
        void IUpdatableGuildChannel.SetNewPermissionsOverwrire(List<Overwrite> overwrite)
        {
            permissionsOverwrite = overwrite;
        }
        void IUpdatableGuildChannel.SetNewPosition(int position)
        {
            Position = position;
        }
        #endregion
        void IGuildChannel.UpdateChannelGuildId(string guildId)
        {
            GuildIdentifier = guildId;
        }
        #region Ctor's
        internal ChannelCategory(string id,
                                  ChannelType type,
                                  string name,
                                  int position,
                                  List<Overwrite> permissionsOverwrite,
                                  bool nsfw,
                                  string parentId)
            : base(id, type)
        {
        }
        internal ChannelCategory(ChannelType type)
            : base(type) { }
        #endregion
    }
}