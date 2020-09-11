using Newtonsoft.Json;

namespace Gateway.Entities.Channels.DM
{
    internal class GroupDMTextChannel : DMChannel, ITextChannel, IGroupDMTextChannel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; private set; }
        #region Ctor's
        internal GroupDMTextChannel()
            : base(ChannelType.GroupDirectMessage) { }
        #endregion
    }
}
