using Newtonsoft.Json;
using System;

namespace DiscordDataObjects.Users.Activities.Listening
{
    internal class ListeningActivity : Activity, IListeningActivity
    {
        public DateTime? StartedAt
        {
            get
            {
                if (timestamps.Start is null)
                {
                    return null;
                }
                return DateTimeOffset.FromUnixTimeMilliseconds(timestamps.Start.Value).LocalDateTime;
            }
        }
        public DateTime? EndedAt
        {
            get
            {
                if (timestamps.End is null)
                {
                    return null;
                }
                return DateTimeOffset.FromUnixTimeMilliseconds(timestamps.End.Value).LocalDateTime;
            }
        }
        [JsonProperty(PropertyName = "details")]
        public string Song { get; private set; }
        public string Album => assets.LargeText;
        internal string AlbumImageId => assets.LargeImage;
        [JsonProperty(PropertyName = "state")]
        public string Band { get; private set; }
        public string PartyIdentifier => party.Identifier;

        [JsonProperty(PropertyName = "party")]
        private RichPresenceActivityParty party;
        [JsonProperty(PropertyName = "assets")]
        private RichPresenceActivityAssets assets;

        internal ListeningActivity() { }
        internal ListeningActivity(string name,
                                   string song, 
                                   string band, 
                                   RichPresenceActivityParty party, 
                                   RichPresenceActivityAssets assets) 
            : base(name)
        {
            Type = ActivityType.Listening;
            Song = song;
            Band = band;
            this.party = party;
            this.assets = assets;
        }
        // TODO: поля
        // - sync_id
        // - session_id
        // - id
        // - small assets
    }
}
