using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordDataObjects.Users
{
    public interface IUser
    {
        bool Verified { get; }
        bool MFA { get; }
        string Identifier { get; }
        string eMail { get; }
        string Username { get; }
        string Discriminator { get; }
        string FullName { get; }
        bool IsBot { get; }
        string AvatarIdentifier { get; }
        string Locale { get; }
        string Mention { get; }
        bool System { get; }
        PremiumType PremiumType { get; }
        UserFlags Flags { get; }
        UserFlags PublicFlags { get; }
    }
    public enum PremiumType
    {
        None = 0,
        NitroClassic = 1,
        Nitro = 2
    }
    [Flags]
    public enum UserFlags
    {
        None = 0,
        DiscordEmployee = 1 << 0,
        DiscordPartner = 1 << 1,
        HypeSquadEvents = 1 << 2,
        BugHunterLevel1 = 1 << 3,
        HouseBravery = 1 << 6,
        HouseBrilliance = 1 << 7,
        HouseBalance = 1 << 8,
        EarlySupporter = 1 << 9,
        TeamUser = 1 << 10,
        System = 1 << 12,
        BugHunterLevel2 = 1 << 14,
        VerifiedBot = 1 << 16,
        VerifiedBotDeveloper = 1 << 17
    }
}
