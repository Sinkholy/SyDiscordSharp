namespace Gateway.Entities.Emojis
{
    public interface IGuildEmoji : IEmoji
    {
        Role[] Roles { get; }
        bool RequireColons { get; }
        bool Managed { get; }
        bool Available { get; }
        string Mention { get; }
    }
}
