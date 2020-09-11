using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities.Channels
{
    [JsonConverter(typeof(IChannelConverter))]
    public interface IChannel
    {
        string Identifier { get; }
        ChannelType Type { get; }
    }

    internal class IChannelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IChannel);
        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            string jsonString = json.ToString();
            ChannelType type = Channel.GetChannelType(json["type"].ToString());
            IChannel result;
            switch (type)
            {
                case ChannelType.DirectMessage:
                    result = new DMTextChannel(type);
                    break;
                case ChannelType.GroupDirectMessage:
                    result = new GroupDMTextChannel(type);
                    break;
                case ChannelType.GuildCategory:
                    result = new ChannelCategory(type);
                    break;
                case ChannelType.GuildNews:
                    result = new GuildNewsChannel(type);
                    break;
                case ChannelType.GuildStore:
                    result = new GuildStoreChannel(type);
                    break;
                case ChannelType.GuildText:
                    result = new GuildTextChannel(type);
                    break;
                case ChannelType.GuildVoice:
                    result = new GuildVoiceChannel(type);
                    break;
                default:
                    throw new Exception("Unknown channel type"); //TODO : адекватное исключение
            }
            JsonConvert.PopulateObject(jsonString, result);
            return result;
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) 
            => throw new NotImplementedException();
    }
}
