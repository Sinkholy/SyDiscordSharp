using Gateway.Entities.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Text
{
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class GuildTextChannelBase : TextChannel, IGuildChannel
    {
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        internal string Name { get; private set; }
        [JsonProperty(PropertyName = "position")]
        internal int Position { get; private set; }
        [JsonProperty(PropertyName = "permission_overwrites")]
        internal List<Overwrite> PermissionsOverwrite { get; private set; }
        [JsonProperty(PropertyName = "nsfw")]
        internal bool NSFW { get; private set; }
        [JsonProperty(PropertyName = "parent_id")]
        internal string ParentIdentifier { get; private set; }

        public void UpdateChannelGuildId(IGuild guild) => GuildIdentifier = guild.Identifier;
        public override string UpdateChannel(IChannel newChannelInfo) 
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.UpdateChannel(newChannelInfo));
            GuildTextChannelBase newChannel = newChannelInfo as GuildTextChannelBase;
            if (newChannel is null)
            {
                DiscordGatewayClient.RaiseLog("Handling channel updated event. Cannot cast to GuildTextChannelBase");
                return string.Empty;
            }
            else
            {
                if(Name != newChannel.Name)
                {
                    Name = newChannel.Name;
                    result.Append("Name | ");
                }
                if(NSFW != newChannel.NSFW)
                {
                    NSFW = newChannel.NSFW;
                    result.Append("NSFW | ");
                }
                if(ParentIdentifier != newChannel.ParentIdentifier)
                {
                    ParentIdentifier = newChannel.ParentIdentifier;
                    result.Append("ParentIdentifier | ");
                }
                if(Position != newChannel.Position)
                {
                    Position = newChannel.Position;
                    result.Append("Position | ");
                }
                if(PermissionsOverwrite.Count != newChannel.PermissionsOverwrite.Count)
                {
                    if (PermissionsOverwrite.Count > newChannel.PermissionsOverwrite.Count)
                    {
                        int count = PermissionsOverwrite.Count - newChannel.PermissionsOverwrite.Count;
                        result.Append($"{count} permission(s) removed | ");
                        PermissionsOverwrite.RemoveRange(PermissionsOverwrite.Count - count, count);
                    }
                    else
                    {
                        int count = newChannel.PermissionsOverwrite.Count - PermissionsOverwrite.Count;
                        result.Append($"{count} permission(s) added | ");
                        PermissionsOverwrite.AddRange(newChannel.PermissionsOverwrite.GetRange(PermissionsOverwrite.Count, count));
                    }
                }
                else
                {
                    for (int i = 0; i < PermissionsOverwrite.Count; i++)
                    {
                        if(PermissionsOverwrite[i].Allow != newChannel.PermissionsOverwrite[i].Allow)
                        {
                            result.Append("Permission changed | ");
                            PermissionsOverwrite[i].Allow = newChannel.PermissionsOverwrite[i].Allow;
                        }
                        if(PermissionsOverwrite[i].Deny != newChannel.PermissionsOverwrite[i].Deny)
                        {
                            result.Append("Permission changed | ");
                            PermissionsOverwrite[i].Deny = newChannel.PermissionsOverwrite[i].Deny;
                        }
                    }
                }
            }
            return result.ToString();
        }
        #region Ctor's
        internal GuildTextChannelBase(string id,
                                      ChannelType type,
                                      string lastMessageId,
                                      string guildId,
                                      string name,
                                      int position,
                                      List<Overwrite> permissionsOverwrite,
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
