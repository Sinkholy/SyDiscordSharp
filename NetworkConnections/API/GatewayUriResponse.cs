using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class GatewayInfo
    {
        public Uri Uri { get; private set; }
        public int Shards { get; private set; }
        internal SessionStartLimit Session { get; private set; }
        public GatewayInfo(Uri url, int shards, SessionStartLimit sessionStartLimit)
        {
            Uri = url;
            Shards = shards;
            Session = sessionStartLimit;
        }
        public class SessionStartLimit
        {
            public int Total { get; private set; }
            public int Remaining { get; private set; }
            public TimeSpan ResetAfter { get; private set; }
            public int MaxConcurrency { get; private set; }
            public SessionStartLimit(int total, int remaining, TimeSpan resetAfter, int concurrency)
            {
                Total = total;
                Remaining = remaining;
                ResetAfter = resetAfter;
                MaxConcurrency = concurrency;
            }
        }
    }
}
