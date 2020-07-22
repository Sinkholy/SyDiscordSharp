using Gateway.Entities.Guilds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Voice
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GuildVoiceChannel : Channel, IVoiceChannel, IGuildChannel
    {
        [JsonProperty(PropertyName = "bitrate")]
        internal int Bitrate;
        [JsonProperty(PropertyName = "rate_limit_per_user")]
        internal int UserLimit;
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        internal string Name;
        [JsonProperty(PropertyName = "nsfw")]
        internal bool NSFW;
        [JsonProperty(PropertyName = "position")]
        internal int Position;
        [JsonProperty(PropertyName = "permission_overwrites")]
        internal Overwrite[] PermissionsOverwrite;
        [JsonProperty(PropertyName = "parent_id")]
        internal string ParentIdentifier;
        public override string UpdateChannel(IChannel channelNewInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.UpdateChannel(channelNewInfo));
            GuildVoiceChannel newChannel = channelNewInfo as GuildVoiceChannel;
            if (newChannel is null)
            {
                DiscordGatewayClient.RaiseLog("Handling channel updated event. Cannot cast to GuildVoiceChannel");
                return "";
            }
            else
            {
                if (Bitrate != newChannel.Bitrate)
                {
                    Bitrate = newChannel.Bitrate;
                    result.Append("Bitrate |");
                }
                if (Name != newChannel.Name)
                {
                    Name = newChannel.Name;
                    result.Append("Name |");
                }
                if (NSFW != newChannel.NSFW)
                {
                    NSFW = newChannel.NSFW;
                    result.Append("NSFW |");
                }
                if (ParentIdentifier != newChannel.ParentIdentifier)
                {
                    ParentIdentifier = newChannel.ParentIdentifier;
                    result.Append("ParrentIdentifier |");
                }
                if (Position != newChannel.Position)
                {
                    Position = newChannel.Position;
                    result.Append("Position |");
                }
                if (UserLimit != newChannel.UserLimit)
                {
                    UserLimit = newChannel.UserLimit;
                    result.Append("Position |");
                }
                if (UserLimit != newChannel.UserLimit)
                {
                    UserLimit = newChannel.UserLimit;
                    result.Append("UserLimit |");
                }
                if (GuildIdentifier != newChannel.GuildIdentifier)
                {
                    GuildIdentifier = newChannel.GuildIdentifier;
                    result.Append("GuildId |");
                }
                //TODO : PermissionOverwrites
            }
            return result.ToString();
        }
        #region IGuildChannel implementation
        public void UpdateChannelGuildId(IGuild guild) => GuildIdentifier = guild.Identifier;
        #endregion
        #region IVoiceChannel implementation
        public void JoinChannel() { }
        #endregion
        #region Ctor's
        internal GuildVoiceChannel(string id,
                                   ChannelType type,
                                   int bitrate,
                                   int userLimit,
                                   string guildId,
                                   string name,
                                   bool nsfw,
                                   int position,
                                   Overwrite[] permissionsOverwrite,
                                   string parentId)
            : base(id, type)
        {
            Bitrate = bitrate;
            UserLimit = userLimit;
            GuildIdentifier = guildId;
            Name = name;
            NSFW = nsfw;
            Position = position;
            PermissionsOverwrite = permissionsOverwrite;
            ParentIdentifier = parentId;
        }
        internal GuildVoiceChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}
