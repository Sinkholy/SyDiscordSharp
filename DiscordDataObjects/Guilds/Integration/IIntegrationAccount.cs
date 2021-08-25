using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Integration
{
    public interface IIntegrationAccount
    {
        string Identifier { get; }
        string Name { get; }
    }
}
