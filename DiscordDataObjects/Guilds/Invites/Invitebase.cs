using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDataObjects.Guilds.Invite
{
    public class InviteBase : IInvite
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; private protected set; }

        [JsonProperty(PropertyName = "guild_id")]
        public string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "channel_id")]
        internal protected string ChannelIdentifier { get; private set; }
    }
}
