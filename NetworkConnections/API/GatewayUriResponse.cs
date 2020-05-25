using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Http
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GatewayInfo
    {
        public Uri Uri { get; private set; }
        public int Shards { get; private set; }
        [JsonProperty(propertyName: "session_start_limit")]
        internal SessionStartLimit Session { get; private set; }
        [JsonConstructor]
        public GatewayInfo(string url, int shards)
        {
            this.Uri = new Uri(url);
            this.Shards = shards;
        }
        [JsonObject(MemberSerialization.OptIn)]
        internal sealed class SessionStartLimit
        {
            public int Total { get; private set; }
            public int Remaining { get; private set; }
            public TimeSpan ResetAfter { get; private set; }
            public int MaxConcurrency { get; private set; }
            [JsonConstructor]
            internal SessionStartLimit(int total, int remaining, int resetAfter, int concurrency)
            {
                this.Total = total;
                this.Remaining = remaining;
                this.MaxConcurrency = concurrency;
                if (resetAfter == 0)
                {
                    this.ResetAfter = TimeSpan.Zero;
                }
                else
                {
                    this.ResetAfter = TimeSpan.FromSeconds(resetAfter);
                }
            }
        }
    }
}
