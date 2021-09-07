using Newtonsoft.Json;
using System;

namespace DiscordDataObjects.Users.Activities.Gaming
{
    internal class GamingActivity : Activity, IGamingActivity
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
        [JsonProperty(PropertyName = "application_id")]
        public string ApplicationIdentifier { get; private set; }
        internal GamingActivity() { }
        internal GamingActivity(string name, string appId = null) 
            : base(name)
        {
            ApplicationIdentifier = appId;
            Type = ActivityType.Game;
        }
    }
}
