using Newtonsoft.Json;

namespace DiscordDataObjects.Users.Activities
{
    internal class RichPresenceActivityAssets
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
}
