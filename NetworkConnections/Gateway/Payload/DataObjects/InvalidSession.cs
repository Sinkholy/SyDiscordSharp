using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Payload.DataObjects
{
    internal class InvalidSession : IGatewayDataObject
    {
        internal bool Resumable { get; private set; }
        internal InvalidSession(bool resumable)
        {
            Resumable = resumable;
        }
    }
}
