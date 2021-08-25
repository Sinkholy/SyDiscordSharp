namespace Gateway.Entities.Channels
{
    public interface IUpdatableChannel
    {
        /// <summary>
        /// Only two types of channels can be exchanged: News to Text \ Text to News;
        /// </summary>
        /// <param name="type"></param>
        void UpdateChannelType(ChannelType type);
    }
}
