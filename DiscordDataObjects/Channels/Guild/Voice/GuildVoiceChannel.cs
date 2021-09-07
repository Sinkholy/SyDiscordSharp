using DiscordDataObjects.Channels.Guild.IUpdatable;
using DiscordDataObjects.VoiceSession;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordDataObjects.Channels.Guild.Voice
{ 
    [JsonObject(MemberSerialization.OptIn)]
    internal class GuildVoiceChannel : GuildChannel, IVoiceChannel, IGuildVoiceChannel, IUpdatableGuildVoiceChannel
    {
        [JsonProperty(PropertyName = "bitrate")]
        public int Bitrate { get; private set; }
        [JsonProperty(PropertyName = "user_limit")]
        public int UserLimit { get; private set; }
        public IReadOnlyCollection<IVoiceSession> ActiveVoiceSessions
            => activeVoiceSessionsEnumerable.ToList();
        public IReadOnlyCollection<string> UsersInChannel
            => activeVoiceSessionsEnumerable.Select(x => x.UserIdentifier).ToList();

        internal IEnumerable<IVoiceSession> activeVoiceSessionsEnumerable { get; set; }

        #region IUpdatableGuildVoiceChannel implementation
        void IUpdatableGuildVoiceChannel.SetNewBitrate(int bitrate)
        {
            if(bitrate < 8000)
            {
                throw new ArgumentOutOfRangeException("Bitrate must be greater or equeal to 8000kbps");
            }
            Bitrate = bitrate;
        }
        void IUpdatableGuildVoiceChannel.SetNewUserLimit(int limit)
        {
            if(limit < 0 || limit > 99)
            {
                throw new ArgumentOutOfRangeException("User limit must be in range 0 - 99");
            }
            UserLimit = limit;
        }
        #endregion
        #region Ctor's
        internal GuildVoiceChannel(string guildId,
                                 string name,
                                 bool nsfw,
                                 int position,
                                 List<PermissionOverwrite> permissionOverwrites,
                                 int bitrate,
                                 int userLimit,
                                 string categoryId = null)
            : base(ChannelType.GuildVoice, guildId, name, nsfw, position, permissionOverwrites, categoryId)
        {
            UserLimit = userLimit;
            Bitrate = bitrate;
        }
        internal GuildVoiceChannel()
            : base(ChannelType.GuildVoice) { }
        #endregion
    }
}
