using Gateway.Entities.Channels.Text;
using Gateway.Entities.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Gateway.Entities.Message
{
    [JsonConverter(typeof(IMessageConverter))]
    public interface IMessage
    {
        string Identifier { get; }
        string GuildIdentifier { get; }
        string ChannelIdentifier { get; }
        MessageType Type { get; }
        MessageFlags Flags { get; }
        string Content { get; }
        DateTime SentTime { get; }
        DateTime? EditTime { get; }
        bool TTS { get; }
        bool MentionEveryone { get; }
        MessageAttachment[] Attachments { get; }
        string Nonce { get; }
        bool Pinned { get; }
        string WebhookIdentifier { get; }
        IReadOnlyCollection<Reaction> Reactions { get; }
        IReadOnlyCollection<ChannelMention> MentionedChannels { get; }
        IReadOnlyCollection<string> MentionedRoles { get; }
        IReadOnlyCollection<IUser> MentionedUsers { get; }
        IUser Author { get; }
        bool IsWebhook { get; }
    }

    public enum MessageType : byte
    {
        Default,
        RecipientAdd,
        RecipientRemove,
        Call,
        ChannelNameChange,
        ChannelIconChange,
        ChannelPinnedMessage,
        GuildMemberJoin,
        UserPremiumGuildSubscription,
        UserPremiumGuildSubscriptionTier1,
        UserPremiumGuildSubscriptionTier2,
        UserPremiumGuildSubscriptionTier3,
        ChannelFollowAdd,
        GuildDiscoveryDisqualified = 14,
        GuildDiscoveryRequalified = 15
    }
    [Flags]
    public enum MessageFlags : int
    {
        Crossposted = 1 << 0,
        IsCrossposted = 1 << 1,
        SuppressEmbeds = 1 << 2,
        SourceMessageDeleted = 1 << 3,
        Urgent = 1 << 4
    }
    public class MessageAttachment //TAI : хранить адресс как Uri, а не как строку
    {
        [JsonProperty(PropertyName = "id")]
        internal string Identifier;
        [JsonProperty(PropertyName = "filename")]
        internal string FileName;
        [JsonProperty(PropertyName = "size")]
        internal int Size;
        [JsonProperty(PropertyName = "url")]
        internal string Url;
        [JsonProperty(PropertyName = "proxy_url")]
        internal string ProxyUrl;
        /// <summary>
        /// Only passed if file == image
        /// </summary>
        [JsonProperty(PropertyName = "height")]
        internal int Height;
        /// <summary>
        /// Only passed if file == image
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        internal int Width;
    }

    internal class IMessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IMessage);
        }
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            IMessage result = null;
            bool defined = false;
            if (json.TryGetValue("embeds", out JToken embedsToken))
            {
                if (embedsToken.HasValues)
                {
                    defined = true;
                    result = json.ToObject<IEmbeddedMessage>();
                }
            }
            if (json.TryGetValue("activity", out JToken activityToken))
            {
                if (activityToken.HasValues)
                {
                    defined = true;
                    result = json.ToObject<IActivityMessage>();
                }
            }
            if (json.TryGetValue("message_reference", out JToken referenceToken))
            {
                if (referenceToken.HasValues)
                {
                    defined = true;
                    result = json.ToObject<ICrossPostedMessage>();
                }
            }
            if (!defined)
            {
                result = new Message();
                JsonConvert.PopulateObject(json.ToString(), result);
            }
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
