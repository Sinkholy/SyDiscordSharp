using Gateway.DataObjects.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects
{
    internal class Presence
    {
        User User;
        Role[] Roles;
        Activity Game;
        string GuildIdentifier;
        string Status;
        Activity[] Activities;
        ClientStatus ClientState;
        DateTime PremiumSince;
        string Nickname;


        internal class ClientStatus
        {
            internal PlatformType Platform;
            internal ClientState State;
            internal enum PlatformType : byte
            {
                Desktop,
                Mobile,
                Web
            }
            internal enum ClientState : byte
            {
                Online,
                Idle,
                DoNotDisturb
            }
        }
    }
}