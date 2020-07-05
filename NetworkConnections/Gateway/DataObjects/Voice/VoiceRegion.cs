using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Voice
{
    internal class VoiceRegion
    {
        string Identifier,
               Name;
        bool Vip,
             Optimal,
             Deprecated,
             Custom;
    }
}
