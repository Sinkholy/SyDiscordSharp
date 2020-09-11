using Gateway.Entities.Users;

namespace Gateway.Entities.Channels.DM
{
    public interface IDMChannel : IChannel
    {
        string ApplicationIdentifier { get; }
        string LastMessageIdentifier { get; }
        string OwnerIdentifier { get; }
        IUser[] Recipients { get; }
    }
}
