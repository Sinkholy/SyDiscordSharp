using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Emojis
{
    public interface IEmoji
    {
        string Identifier { get; }
        string Name { get; }
        bool Animated { get; }
    }
}
