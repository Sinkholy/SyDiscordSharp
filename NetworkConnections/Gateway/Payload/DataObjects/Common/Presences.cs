using Gateway.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.PayloadCommon
{
    [JsonObject(MemberSerialization.OptIn)]
    class Presences
    {
        [JsonProperty(PropertyName = "game", Order = 0)]
        public Game Game;
        [JsonProperty(propertyName: "status", Order = 1)]
        public UserStatus Status;
        [JsonProperty(propertyName: "since", Order = 2)]
        public TimeSpan Since;
        [JsonProperty(propertyName: "afk", Order = 3)]
        public bool Afk;
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class Game
    {
        [JsonProperty(PropertyName = "name", Order = 0)]
        public string Name;
        [JsonProperty(PropertyName = "type", Order = 1)]
        public GameType Type;
    }
}