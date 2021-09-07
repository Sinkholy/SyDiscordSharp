namespace DiscordDataObjects.Channels.DM
{
    internal class DMTextChannel : DMChannel, ITextChannel, IDMTextChannel
    {
        #region Ctor's
        internal DMTextChannel()
            : base(ChannelType.DirectMessage) { }
        #endregion
    }
}