using Gateway.DataObjects.Voice;
using Gateway.Entities.Guilds;
using Gateway.Entities.Users;
using Gateway.Entities.VoiceSession;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels.Guild.Voice
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class GuildVoiceChannel : Channel, IVoiceChannel, IGuildChannel
    {
        [JsonProperty(PropertyName = "bitrate")]
        public int Bitrate { get; private set; }
        [JsonProperty(PropertyName = "rate_limit_per_user")]
        public int UserLimit { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "nsfw")]
        public bool NSFW { get; private set; }
        [JsonProperty(PropertyName = "position")]
        public int Position { get; private set; }
        [JsonProperty(PropertyName = "parent_id")]
        public string CategoryIdentifier { get; private set; }
        public IReadOnlyCollection<Overwrite> PermissionsOverwrite => permissionsOverwrite;
        public IReadOnlyCollection<IVoiceSession> ActiveVoiceSessions
            => activeVoiceSessionsEnumerable.ToList();
        public IReadOnlyCollection<string> UsersInChannel
            => activeVoiceSessionsEnumerable.Select(x => x.UserIdentifier).ToList();
        #region IUpdatableChannel implementation
        public override string UpdateChannel(IChannel channelNewInfo)
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.UpdateChannel(channelNewInfo));
            GuildVoiceChannel newChannel = channelNewInfo as GuildVoiceChannel;
            if (newChannel is null)
            {
                // TODO : инструмент логирования ("Handling channel updated event. Cannot cast to GuildVoiceChannel");
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
                //if (ParentIdentifier != newChannel.ParentIdentifier)
                //{
                //    ParentIdentifier = newChannel.ParentIdentifier;
                //    result.Append("ParrentIdentifier |");
                //}
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
        #endregion
        #region IGuildChannel implementation
        public void UpdateChannelGuildId(string guildId) => GuildIdentifier = guildId;
        #endregion
        #region IVoiceChannel implementation
        #endregion
        [JsonProperty(PropertyName = "permission_overwrites")]
        private List<Overwrite> permissionsOverwrite;
        internal IEnumerable<IVoiceSession> activeVoiceSessionsEnumerable { get; set; }
        #region Ctor's
        internal GuildVoiceChannel(string id,
                                   ChannelType type,
                                   int bitrate,
                                   int userLimit,
                                   string guildId,
                                   string name,
                                   bool nsfw,
                                   int position,
                                   List<Overwrite> permissionsOverwrite,
                                   string parentId)
            : base(id, type)
        {
            Bitrate = bitrate;
            UserLimit = userLimit;
            GuildIdentifier = guildId;
            Name = name;
            NSFW = nsfw;
            Position = position;
            this.permissionsOverwrite = permissionsOverwrite;
            CategoryIdentifier = parentId;
        }
        internal GuildVoiceChannel(ChannelType type)
            : base(type) { }
        #endregion
    }
}
