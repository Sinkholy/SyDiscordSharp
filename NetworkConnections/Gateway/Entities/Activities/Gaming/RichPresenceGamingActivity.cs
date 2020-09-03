using Newtonsoft.Json;

namespace Gateway.Entities.Activities.Gaming
{
    internal class RichPresenceGamingActivity : GamingActivity, IRichPresenceGamingActivity
    {
        [JsonProperty(PropertyName = "state")]
        public string State { get; private set; } 
        public string PartyIdentifier => party.Identifier;
        public int? PartySize => party.CurrentSize;
        public int? PartySizeLimit => party.MaxSize;
        [JsonProperty(PropertyName = "details")]
        public string Details { get; private set; }
        public string ImageName => assets.LargeText;
        public string ImageIdentifier => assets.LargeImage;

        [JsonProperty(PropertyName = "assets")]
        private RichPresenceActivityAssets assets;
        [JsonProperty(PropertyName = "party")]
        private RichPresenceActivityParty party;
        //[JsonProperty(PropertyName = "instance")]
        //internal bool? Instance { get; private set; }
        //[JsonProperty(PropertyName = "secrets")]
        //internal ActivitySecrets Secrets { get; private set; }

        // TODO: поля
        // - session_id
        // - assets (small)
    }
}
