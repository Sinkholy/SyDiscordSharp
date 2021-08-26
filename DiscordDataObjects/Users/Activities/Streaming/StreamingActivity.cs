using Newtonsoft.Json;
using System;

namespace Gateway.Entities.Activities.Streaming
{
    internal class StreamingActivity : Activity, IStreamingActivity
    {
        public Uri Url => new Uri(uri);
        public StreamingPlatform Platform { get; private set; }
        [JsonProperty(PropertyName = "details")]
        public string StreamName { get; private set; }
        [JsonProperty(PropertyName = "state")]
        public string Game { get; private set; }
        public string StreamImagePreview => assets.LargeImage;

        [JsonProperty(PropertyName = "assets")]
        private RichPresenceActivityAssets assets;
        [JsonProperty(PropertyName = "url")]
        private string uri;

        internal StreamingActivity() { }
        internal StreamingActivity(
                                   StreamingPlatform platform, 
                                   string streamName, 
                                   string uri, 
                                   RichPresenceActivityAssets assets) 
            : base()
        {
            Type = ActivityType.Streaming;
            Name = streamName;
            Platform = platform;
            StreamName = streamName;
            this.uri = uri;
            this.assets = assets;
        }
    }
}
