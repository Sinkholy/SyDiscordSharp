using Gateway.Entities.Activities.Custom;
using Gateway.Entities.Activities.Gaming;
using Gateway.Entities.Activities.Listening;
using Gateway.Entities.Activities.Streaming;
using Gateway.Entities.Emojis;
using Newtonsoft.Json;
using System;

namespace DiscordDataObjects.Users.Activities
{
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class Activity : IActivity
    {
        #region IActivity implementation
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private protected set; }
        public DateTime CreatedAt 
            => DateTimeOffset.FromUnixTimeMilliseconds(createdAtUnix).LocalDateTime;
        [JsonProperty(PropertyName = "type")]
        public ActivityType Type { get; private protected set; }
        [JsonProperty(PropertyName = "flags")]
        public ActivityFlags? Flags { get; private set; }
        #endregion

        [JsonProperty(PropertyName = "created_at")]
        private long createdAtUnix;
        [JsonProperty(PropertyName = "timestamps")]
        private protected Timestamps timestamps;

        internal static IStreamingActivity CreateStreamingActivity(
                                                                   StreamingPlatform platform,
                                                                   string streamName,
                                                                   string uri,
                                                                   RichPresenceActivityAssets assets) //TODO : дотестить как оно вообще работает
        {
            return new StreamingActivity(platform, streamName, uri, assets);
        }
        internal static IGamingActivity CreateGamingActivity(string name, string appId = null)
        {
            return new GamingActivity(name, appId);
        }
        internal static ICustomStatusActivity CreateCustomStatusActivity(string name, string text, IEmoji emoji = null)
        {
            return new CustomStatusActivity(name, text, emoji);
        }
        internal static IListeningActivity CreateListeningActivity(string name, // TODO: интеграция со спотифи https://just-some-bots.github.io/MusicBot/using/spotify/
                                                                   string song,
                                                                   string band,
                                                                   RichPresenceActivityParty party,
                                                                   RichPresenceActivityAssets assets)
        {
            return new ListeningActivity(name, song, band, party, assets);
        }

        internal Activity() { }
        internal Activity(string name) 
        {
            Name = name;
        }
        private protected class Timestamps
        {
            [JsonProperty(PropertyName = "start")]
            internal long? Start;
            [JsonProperty(PropertyName = "end")]
            internal long? End;
        }
    }

    [Flags]
    public enum ActivityFlags
    {
        Instance = 1 << 0,
        Join = 1 << 1,
        Spectate = 1 << 2,
        JoinRequest = 1 << 3,
        Sync = 1 << 4,
        Play = 1 << 5
    }
    public enum ActivityType : byte
    {
        Game = 0,
        Streaming = 1,
        Listening = 2,
        IIRC = 3,
        Custom = 4
    }
}
