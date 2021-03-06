﻿using Gateway.Entities.Emojis;
using Newtonsoft.Json;

namespace Gateway.Entities.Activities.Custom
{
    internal class CustomStatusActivity : Activity, ICustomStatusActivity
    {
        [JsonProperty(PropertyName = "state")]
        public string Status { get; private set; }
        [JsonProperty(PropertyName = "emoji")]
        public IEmoji Emoji { get; private set; }
        internal CustomStatusActivity() { }
        internal CustomStatusActivity(string name, string text, IEmoji emoji = null) 
            : base(name)
        {
            Type = ActivityType.Custom;
            Status = text;
            Emoji = emoji;
        }
    }
}
