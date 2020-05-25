using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gateway.PayloadCommon;
using Gateway.Enums;
using Newtonsoft.Json;

namespace Gateway.Payload.DataObjects
{
    class UpdateStatus
    {
        [JsonProperty(PropertyName = "since", Order = 0)]
        public int since;
        [JsonProperty(PropertyName = "game", Order = 1)]
        public Game game;
        [JsonProperty(PropertyName = "status", Order = 2)]
        public UserStatus status;
        [JsonProperty(PropertyName = "afk", Order = 3)]
        public bool AFK;

        public UpdateStatus(int since, Game game, UserStatus status, bool afk)
        {
            this.since = since;
            this.game = game;
            this.status = status;
            this.AFK = afk;
        }
    }
}
