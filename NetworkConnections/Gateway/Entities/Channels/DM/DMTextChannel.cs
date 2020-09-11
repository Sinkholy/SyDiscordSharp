namespace Gateway.Entities.Channels.DM
{
    internal class DMTextChannel : DMChannel, IDMTextChannel
    {
        #region Ctor's
        internal DMTextChannel()
            : base(ChannelType.DirectMessage) { }
        #endregion
    }
}