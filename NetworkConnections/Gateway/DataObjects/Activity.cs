using Gateway.DataObjects.Emojis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects
{
    internal class Activity
    {
        [JsonProperty(PropertyName = "name")]
        internal string Name { get; private set; }
        [JsonProperty(PropertyName = "type")]
        internal ActivityType Type { get; private set; }
        /// <summary>
        /// Url to Activity. Only passed when Activity.Type == 1 (Streaming)
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        internal string Url { get; private set; } //Only Type == 1(streaming)
        internal DateTime CreatedUtc { get; private set; }
        internal DateTime StartedUtc { get; private set; }
        internal DateTime? EndedUtc { get; private set; }
        [JsonProperty(PropertyName = "application_id")]
        internal string ApplicationIdentifier { get; private set; }
        [JsonProperty(PropertyName = "details")]
        internal string Details { get; private set; }
        [JsonProperty(PropertyName = "state")]
        internal string PartyStatus { get; private set; }
        [JsonProperty(PropertyName = "emoji")]
        internal Emoji CustomStatusEmoji { get; private set; }
        [JsonProperty(PropertyName = "party")]
        internal ActivityParty Party { get; private set; }
        [JsonProperty(PropertyName = "assets")]
        internal ActivityAssets Assets { get; private set; }
        [JsonProperty(PropertyName = "instance")]
        internal bool Instance { get; private set; }
        [JsonProperty(PropertyName = "secrets")]
        internal ActivitySecrets Secrets { get; private set; }
        [JsonProperty(PropertyName = "flags")]
        internal ActivityFlags Flags { get; private set; }

        [JsonProperty(PropertyName = "created_at")]
        private int createdAtUnix;
        [JsonProperty(PropertyName = "timestamps")]
        private Timestamps timestamps;

        private DateTime ConvertDateTimeFromUnix(int seconds)
        {
            DateTime original = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return original.AddSeconds(seconds);
        }
        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            CreatedUtc = ConvertDateTimeFromUnix(createdAtUnix);
            StartedUtc = ConvertDateTimeFromUnix(timestamps.Start);
            EndedUtc = ConvertDateTimeFromUnix(timestamps.End);
        }
        private class Timestamps
        {
            [JsonProperty(PropertyName = "start")]
            internal int Start { get; private set; }
            [JsonProperty(PropertyName = "end")]
            internal int End { get; private set; }
        }
    }
    internal class ActivityParty
    {
        [JsonProperty(PropertyName = "id")]
        internal string Identifier { get; private set; }
        internal int CurrentSize => size[0];
        internal int MaxSize => size[1];

        [JsonProperty(PropertyName = "size")]
        private int[] size;
    }
    internal class ActivityAssets
    {
        [JsonProperty(PropertyName = "large_image")]
        internal string LargeImage { get; private set; }
        [JsonProperty(PropertyName = "large_text")]
        internal string LargeText { get; private set; }
        [JsonProperty(PropertyName = "small_image")]
        internal string SmallImage { get; private set; }
        [JsonProperty(PropertyName = "small_text")]
        internal string SmallText { get; private set; }
    }
    internal class ActivitySecrets
    {
        [JsonProperty(PropertyName = "join")]
        internal string Join { get; private set; }
        [JsonProperty(PropertyName = "spectate")]
        internal string Spectate { get; private set; }
        [JsonProperty(PropertyName = "match")]
        internal string Match { get; private set; }
    }
    [Flags]
    internal enum ActivityFlags
    {
        Instance = 1<<0,
        Join = 1<<1,
        Spectate = 1<<2,
        JoinRequest = 1<<3,
        Sync = 1<<4,
        Play = 1<<5
    }
    internal enum ActivityType : byte //TODO : активити очень под вопросом потому что, пока что, 
                                      //не ясно как оно передается
    {
        Game = 0,
        Streaming = 1,
        Listening = 2,
        Custom = 4
    }
}