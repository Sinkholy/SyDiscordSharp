using Gateway.DataObjects.Emojis;
using Newtonsoft.Json;
using System;

namespace Gateway.DataObjects.Guilds
{
    internal class GuildPreview : IGuild
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; private set; }
        [JsonProperty(PropertyName = "splash")]
        public string Splash { get; private set; }
        [JsonProperty(PropertyName = "discovery_splash")]
        public string DiscoverySplash { get; private set; }
        [JsonProperty(PropertyName = "approximate_member_count")]
        public int ApproximateMemberCount { get; private set; }
        [JsonProperty(PropertyName = "approximate_presence_count")]
        public int ApproximatePresenceCount { get; private set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; private set; }
        [JsonProperty(PropertyName = "emojis")]
        public GuildEmoji[] Emojis { get; private set; }
        [JsonProperty(PropertyName = "features")]
        public GuildFeatures[] Features { get; private set; }
        [JsonProperty(PropertyName = "available")]
        public bool Unavailable { get; private set; }

        public void UpdateGuild(IGuild newGuildInfo)
        {

        }
    }
}