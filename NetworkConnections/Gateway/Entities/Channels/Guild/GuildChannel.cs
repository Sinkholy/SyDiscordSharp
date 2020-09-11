﻿using Gateway.Entities.Channels.Guild.IUpdatable;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gateway.Entities.Channels.Guild
{
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
            Name = name;
        }
        void IUpdatableGuildChannel.SetNewPosition(int position)
        {
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
            GuildIdentifier = guildId;
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
