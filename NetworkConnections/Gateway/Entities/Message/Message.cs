using Gateway.Entities.Channels;
using Gateway.Entities.Channels.Text;
using Gateway.Entities.Emojis;
using Gateway.Entities.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Gateway.Entities.Message
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Message : MessageBase, IMessage, IUpdatableMessage
    {
        #region IMessage implementation
        [JsonProperty(PropertyName = "type")]
        public MessageType Type { get; private set; }
        [JsonProperty(PropertyName = "flags")]
        public MessageFlags Flags { get; private set; }
        [JsonProperty(PropertyName = "content")]
        public string Content { get; private set; } // TODO: приватный сеттер
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime SentTime { get; private set; }
        [JsonProperty(PropertyName = "edited_timestamp")]
        public DateTime? EditTime { get; private set; }
        [JsonProperty(PropertyName = "tts")]
        public bool TTS { get; private set; }
        [JsonProperty(PropertyName = "mention_everyone")]
        public bool MentionEveryone { get; private set; }
        [JsonProperty(PropertyName = "attachments")]
        public MessageAttachment[] Attachments { get; private set; } //TODO : Проверить реакции и аттачменты можно ли их вообще впихнуть
        [JsonProperty(PropertyName = "nonce")]
        public string Nonce { get; private set; }
        [JsonProperty(PropertyName = "pinned")]
        public bool Pinned { get; private set; }
        [JsonProperty(PropertyName = "webhook_id")] //TODO : если будет вебхук объект - вставить сюда
        public string WebhookIdentifier { get; private set; }
        public IReadOnlyCollection<Reaction> Reactions => ReactionsById.Values.ToList();
        public IReadOnlyCollection<ChannelMention> MentionedChannels 
        {
            get
            {
                if (mentionedChannelsReceived is null)
                {
                    mentionedChannelsReceived = new List<ChannelMention>();
                }
                return mentionedChannelsReceived;
            }
        }
        public IReadOnlyCollection<string> MentionedRoles 
        {
            get
            {
                if (mentionedRolesReceived is null)
                {
                    mentionedRolesReceived = new List<string>();
                }
                return mentionedRolesReceived;
            }
        }
        public IReadOnlyCollection<IUser> MentionedUsers 
        {
            get
            {
                if (mentionedUsersReceived is null)
                {
                    mentionedUsersReceived = new List<User>();
                }
                return mentionedUsersReceived;
            }
        }
        public IUser Author => author;
        public bool IsWebhook => WebhookIdentifier != null;
        #endregion
        #region IUpdatableMessage impl
        void IUpdatableMessage.AddReaction(IEmoji emoji)
        {
            if (ReactionsById.TryGetValue(emoji.Identifier, out Reaction react))
            {
                react.IncrementCount();
            }
            else
            {
                Reaction reaction = new Reaction(1, false, emoji); // TODO : Не понимаю как определить поле ME без запроса к HTTP 
                ReactionsById.Add(emoji.Identifier, reaction);
            }
        }
        void IUpdatableMessage.RemoveReaction(IEmoji emoji) 
        {
            if (ReactionsById.TryGetValue(emoji.Identifier, out Reaction react))
            {
                if(react.DecrementCount() == 0)
                {
                    ReactionsById.Remove(emoji.Identifier);
                }
            }
        }
        void IUpdatableMessage.RemoveAllReactions()
        {
            ReactionsById.Clear();
        }
        void IUpdatableMessage.RemoveAllEmojiReactions(IEmoji emoji)
        {
            if (ReactionsById.ContainsKey(emoji.Identifier))
            {
                ReactionsById.Remove(emoji.Identifier);
            }
        }
        #endregion
        [JsonProperty(PropertyName = "mention_channels")]
        private List<ChannelMention> mentionedChannelsReceived;
        [JsonProperty(PropertyName = "mention_roles")]
        private List<string> mentionedRolesReceived;
        [JsonProperty(PropertyName = "mentions")]
        private List<User> mentionedUsersReceived;
        [JsonProperty(PropertyName = "author")]
        private User author;
        private Dictionary<string, Reaction> ReactionsById
        {
            get
            {
                if (_reactionsById is null)
                {
                    _reactionsById = new Dictionary<string, Reaction>();
                }
                return _reactionsById;
            }
            set => _reactionsById = value;
        }
        private Dictionary<string, Reaction> _reactionsById;
        [JsonProperty(PropertyName = "reactions")]
        private Reaction[] reactions;

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            if (reactions != null)
            {
                ReactionsById = new Dictionary<string, Reaction>(capacity: reactions.Length);
                foreach (Reaction react in reactions)
                {
                    ReactionsById.Add(react.Emoji.Identifier, react);
                }
            }
        }

        #region Ctor's
        public Message(string content,
                       string nonce,
                       bool tts = false,
                       MessageAttachment[] attachements = null)
        {
            Content = content;
            Nonce = nonce;
            TTS = tts;
            Attachments = attachements;
        }
        public Message() { }
        #endregion
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ChannelMention
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier;
        [JsonProperty(PropertyName = "guild_id")]
        public string GuildId;
        [JsonProperty(PropertyName = "type")]
        public ChannelType Type;
        [JsonProperty(PropertyName = "name")]
        public string Name;
    }
}