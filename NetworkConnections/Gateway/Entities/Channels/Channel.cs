using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Gateway.Entities.Channels
{
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class Channel : IChannel
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        public ChannelType Type { get; private set; }

        internal static ChannelType GetChannelType(string type) 
            => (ChannelType) Enum.Parse(typeof(ChannelType), type);

        #region Ctor's
        internal Channel(string id, ChannelType type)
        {
            Identifier = id;
            Type = type;
        }
        internal Channel(ChannelType type) 
        {
            Type = type;
        }
        #endregion
    }
    public enum ChannelType : byte
    {
        GuildText = 0,
        DirectMessage = 1,
        GuildVoice = 2,
        GroupDirectMessage = 3,
        GuildCategory = 4,
        GuildNews = 5,
        GuildStore = 6
    }
}
