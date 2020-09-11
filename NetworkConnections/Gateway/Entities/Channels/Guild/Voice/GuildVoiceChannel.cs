using Gateway.Entities.Channels.Guild.IUpdatable;
using Gateway.Entities.VoiceSession;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Gateway.Entities.Channels.Guild.Voice
{
    internal class GuildVoiceChannel : GuildChannel, IGuildVoiceChannel, IUpdatableGuildVoiceChannel
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
            Bitrate = bitrate;
        }
        void IUpdatableGuildVoiceChannel.SetNewUserLimit(int limit)
        {
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
