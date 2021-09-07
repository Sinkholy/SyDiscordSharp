using Gateway.Entities.Emojis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDataObjects.Channels.Message
{
    public class Reaction // TAI: интерефейс?
    {
        [JsonProperty(PropertyName = "count")]
        internal int Count { get; private set; }
        [JsonProperty(PropertyName = "me")]
        internal bool Me { get; private set; }
        [JsonProperty(PropertyName = "emoji")]
        internal IEmoji Emoji { get; private set; }

        internal int IncrementCount()
        {
            return ++Count;
        }
        internal int DecrementCount()
        {
            return --Count;
        }
        internal Reaction() { }
        internal Reaction(int count, bool me, IEmoji emoji)
        {
            Count = count;
            Me = me;
            Emoji = emoji;
        }
    }
}
