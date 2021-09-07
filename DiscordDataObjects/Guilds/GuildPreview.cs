using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordDataObjects.Guilds
{
    internal class GuildPreview
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
        private protected GuildEmoji[] _emojis;
        [JsonProperty(PropertyName = "features")]
        public GuildFeatures[] Features { get; private set; }
        [JsonProperty(PropertyName = "available")]
        public bool Unavailable { get; internal set; }
    }
}