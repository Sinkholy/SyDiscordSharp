using DiscordDataObjects.Guilds.Presences;
using DiscordDataObjects.Users.Activities;

using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using System;

namespace Gateway.Payload.DataObjects
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Identify
    {
        [JsonProperty(propertyName: "token")]
        public string Token { get; private set; }
        [JsonProperty(propertyName: "compress")]
        public bool Compress { get; private set; }
        [JsonProperty(propertyName: "largeThreshold")]
        public int LargeThreshold { get; private set; }
        [JsonProperty(propertyName: "guildSubscriptions")]
        public bool GuildSubscriptions { get; private set; }
        //[JsonProperty(propertyName: "shard")] // TODO: shard
        //public int[] Shard { get; private set; }
        [JsonProperty(propertyName: "presence", Order = 6)]
        public IdentifyPresence Presences { get; private set; }
        [JsonProperty(propertyName: "properties")]
        public IdentifyProperties Properties { get; private set; }
        [JsonProperty(propertyName: "intents")]
        public string Intents { get; private set; }
        public Identify(string token,
                        IdentifyProperties properties,
                        IdentifyIntents intents = null,
                        IdentifyPresence presences = null,
                        int[] shards = null,
                        bool compress = false,
                        int largeThreshold = 50,
                        bool guildSubscriptions = true)
        {
            Presences = presences;
            Properties = properties;
            Token = token;
            //Shard = shards ?? new int[2] { 0, 1 };
            Intents = intents?.Intents;
            Compress = compress;
            LargeThreshold = largeThreshold;
            GuildSubscriptions = guildSubscriptions;
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    internal class IdentifyProperties
    {
        [JsonProperty(propertyName: "$os", Order = 0)]
        public string Os { get; private set; } = Environment.OSVersion.ToString();
        [JsonProperty(propertyName: "$browser", Order = 1)]
        public string Browser { get; private set; }
        [JsonProperty(propertyName: "$device", Order = 2)]
        public string Device { get; private set; }
        public IdentifyProperties(string browser, string device)
        {
            Browser = browser;
            Device = device;
        }
    }
    internal class IdentifyIntents
    {
        internal string Intents { get; private set; }
        internal static IdentifyIntents None = null,
                                        Default = new IdentifyIntents(1 << 0 | 1 << 2 | 1 << 3 |
                                                                      1 << 4 | 1 << 5 | 1 << 6 |
                                                                      1 << 7 | 1 << 9 | 1 << 10 |
                                                                      1 << 11 | 1 << 12 | 1 << 13 |
                                                                      1 << 14),
                                        Custom = new IdentifyIntents(GetCustomValue());
        private IdentifyIntents(int intents) => Intents = intents.ToString();
        private static int GetCustomValue() //TODO : кастомные интенты
        {
            return 0;
		}
    }
    internal class IdentifyPresence
    {
        [JsonProperty(PropertyName = "since")]
        internal string UnixIdleSince { get; private set; }
        [JsonProperty(PropertyName = "game")]
        internal IActivity Game { get; private set; }
        /// <summary>
        /// As far as I can see only online\invisible\dnd\idle can be passed
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        internal string Status { get; private set; }
        [JsonProperty(PropertyName = "afk")]
        internal bool Afk { get; private set; }
        internal IdentifyPresence(UserStatus status, bool afk, string idleSince, IActivity game = null)
        {
            Status = status.ToString().ToLower();
            Game = game;
            UnixIdleSince = idleSince;
            Afk = afk;
        }
    }
}
