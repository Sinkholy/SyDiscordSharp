using Gateway.DataObjects.Emojis;
using Gateway.Payload.EventObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Guilds
{
    [JsonObject(MemberSerialization.OptOut)]
    internal class GuildPreview : IGuild, IEvent
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        internal string Name;
        [JsonProperty(PropertyName = "icon")]
        internal string Icon;
        [JsonProperty(PropertyName = "splash")]
        internal string Splash;
        [JsonProperty(PropertyName = "discovery_splash")]
        internal string DiscoverySplash;
        [JsonProperty(PropertyName = "approximate_member_count")]
        internal int ApproximateMemberCount;
        [JsonProperty(PropertyName = "approximate_presence_count")]
        internal int ApproximatePresenceCount;
        [JsonProperty(PropertyName = "description")]
        internal string Description;
        [JsonProperty(PropertyName = "emojis")]
        internal GuildEmoji[] Emojis;
        [JsonProperty(PropertyName = "features")]
        internal GuildFeatures[] Features;
        [JsonProperty(PropertyName = "available")]
        internal bool Available;

        public void UpdateGuild(IGuild newGuildInfo)
        {

        }
    }
    internal enum GuildFeatures
    {
        InviteSplash,
        VipRegions,
        VanityUrl,
        Verified,
        Partnered,
        Public,
        Commerce,
        News,
        Discoverable,
        Featurable,
        AnimatedIcon,
        Banner,
        PublicDisabled,
        WelcomeScreenEnabled
    }
}