using Gateway.Entities.Users;
using Newtonsoft.Json;

namespace DiscordDataObjects.Channels.DM
{
    internal abstract class DMChannel : Channel, IDMChannel // TODO: могу ли я создавать DM-каналы и "скармливать"
                                                            // их дискорду?
                                                            // if(da) создать соответствующие конструкторы
                                                            // else удалить коммент xd
    {
        [JsonProperty(PropertyName = "recipients")]
        public IUser[] Recipients { get; private set; }
        [JsonProperty(PropertyName = "last_message_id")]
        public string LastMessageIdentifier { get; private set; }
        [JsonProperty(PropertyName = "owner_id")] // TODO: под вопросом - проверить.
        public string OwnerIdentifier { get; private set; }
        [JsonProperty(PropertyName = "application_id")] // смотри на OwnerId
        public string ApplicationIdentifier { get; private set; }

        private protected DMChannel(ChannelType type)
            : base(type) { }
    }
}
