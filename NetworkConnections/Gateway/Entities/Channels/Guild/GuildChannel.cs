using Gateway.Entities.Channels.Guild.IUpdatable;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gateway.Entities.Channels.Guild
{
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class GuildChannel : Channel, IGuildChannel, IUpdatableGuildChannel
    {
        [JsonProperty(PropertyName = "parent_id")]
        public string CategoryIdentifier { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "nsfw")]
        public bool NSFW { get; private set; }
        [JsonProperty(PropertyName = "position")]
        public int Position { get; private set; }
        public IReadOnlyCollection<PermissionOverwrite> PermissionOverwrites => permissionOverwrites;

        [JsonProperty(PropertyName = "permission_overwrites")]
        private List<PermissionOverwrite> permissionOverwrites;

        #region IUpdatableGuildChannel implementation
        void IUpdatableGuildChannel.SetNewName(string name)
        {
            if(name.Length < 2 || name.Length > 100)
            {
                throw new ArgumentOutOfRangeException("Name must be in range 2-100 characters.");
            }
            Name = name;
        }
        void IUpdatableGuildChannel.SetNewPosition(int position)
        {
            if(position < 0)
            {
                throw new ArgumentOutOfRangeException("Position must be greater or equal 0.");
            }
            Position = position;
        }
        void IUpdatableGuildChannel.SetNewNsfw(bool nsfw)
        {
            NSFW = nsfw;
        }
        void IUpdatableGuildChannel.SetNewPermissionsOverwrire(List<PermissionOverwrite> overwrite)
        {
            permissionOverwrites = overwrite;
        }
        void IUpdatableGuildChannel.SetNewCategory(string categoryId)
        {
            CategoryIdentifier = categoryId;
        }
        void IUpdatableGuildChannel.SetNewGuildId(string guildId)
        {
            if(string.IsNullOrWhiteSpace(guildId) || guildId is null)
            {
                throw new ArgumentNullException("Cannot set no guild to channel.");
            }
            GuildIdentifier = guildId;
        }
        void IUpdatableGuildChannel.AddNewPermissionOverwrite(PermissionOverwrite overwrite)
        {
            if(overwrite is null)
            {
                throw new ArgumentNullException("Cannot add empty overwrite to channel.");
            }
            permissionOverwrites.Add(overwrite);
        }
        void IUpdatableGuildChannel.RemovePermissionOverwrite(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("Cannot remove overwrite with null or empty id.");
            }
            PermissionOverwrite overwriteToRemove = permissionOverwrites.Where(x => x.Identifier == id)
                                                                        .SingleOrDefault();
            if(overwriteToRemove != null)
            {
                permissionOverwrites.Remove(overwriteToRemove);
            }
        }
        #endregion
        #region Ctor's
        private protected GuildChannel(ChannelType type,
                              string guildId,
                              string name,
                              bool nsfw,
                              int position,
                              List<PermissionOverwrite> permissionOverwrites,
                              string categoryId = null)
            : base(type)
        {
            GuildIdentifier = guildId;
            Name = name;
            NSFW = nsfw;
            Position = position;
            this.permissionOverwrites = permissionOverwrites;
            CategoryIdentifier = categoryId;
        }
        private protected GuildChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}
