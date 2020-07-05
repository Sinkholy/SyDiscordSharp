using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Users
{
    public interface IUser
    {
        //bool Verified { get; }
        string Username { get; }
        //bool MFA { get; }
        string Identifier { get; }
        //string eMail { get; }
        //string Discriminator { get; }
        //bool IsBot { get; }
        //string AvatarIdentifier { get; }
    }
}
