using Gateway.Entities;
using Gateway.Entities.Emojis;
using Gateway.Entities.Guilds;
using Gateway.Entities.Users;
using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects.Dispatch.DispatchEvents
{
    internal class PresenceUpdatedEvent
    {
        internal IUser User => user as IUser;
        internal IGuild Guild { get; private set; }
        [JsonProperty(PropertyName = "guild_id")]
        internal string GuildIdentifier { get; private set; }
        [JsonProperty(PropertyName = "roles")]
        internal Role[] RolesIdentifiers;
        [JsonProperty(PropertyName = "activity")]
        internal Activity Game { get; private set; }
        [JsonProperty(PropertyName = "activities")]
        internal Activity[] Activities { get; private set; }
        [JsonProperty(PropertyName = "premium_since")]
        internal DateTime PremiumSince { get; private set; }
        [JsonProperty(PropertyName = "status")]
        internal UserStatus Status { get; private set; }
        [JsonProperty(PropertyName = "client_status")]
        internal ClientStatus ClientStatuses { get; private set; }
        internal string Nickname { get; private set; }

        [JsonProperty(PropertyName = "user")]
        private User user;

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            Guild = DiscordGatewayClient.TryToGetGuild(GuildIdentifier);
        }
        /// <summary>
        /// Represents client online states for all platforms
        /// </summary>
        /// <remarks>
        /// Fields are partial UserStatus enum, only can have: Online, Idle, Dnd 
        /// otherwise field isnt present
        /// </remarks>
        internal class ClientStatus
        {
            [JsonProperty(PropertyName = "desktop")]
            internal UserStatus? Desktop { get; private set; }
            [JsonProperty(PropertyName = "mobile")]
            internal UserStatus? Mobile { get; private set; }
            [JsonProperty(PropertyName = "web")]
            internal UserStatus? Web { get; private set; }
        }

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
            Instance = 1 << 0,
            Join = 1 << 1,
            Spectate = 1 << 2,
            JoinRequest = 1 << 3,
            Sync = 1 << 4,
            Play = 1 << 5
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
}
