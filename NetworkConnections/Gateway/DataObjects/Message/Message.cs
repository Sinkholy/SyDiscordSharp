using Gateway.DataObjects.Channels;
using Gateway.DataObjects.Channels.Text;
using Gateway.DataObjects.Emojis;
using Gateway.DataObjects.Guilds;
using Gateway.DataObjects.Roles;
using Gateway.DataObjects.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.DataObjects.Message
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Message : MessageBase
    {
        [JsonProperty(PropertyName = "type")]
        internal MessageType Type;
        [JsonProperty(PropertyName = "flags")]
        internal MessageFlags Flags;
        [JsonProperty(PropertyName = "content")]
        internal string Content;
        [JsonProperty(PropertyName = "timestamp")]
        internal DateTime SentTime;
        [JsonProperty(PropertyName = "edited_timestamp")]
        internal DateTime? EditTime;
        [JsonProperty(PropertyName = "activity")]
        internal MessageActivity Activity;
        [JsonProperty(PropertyName = "tts")]
        internal bool TTS;
        [JsonProperty(PropertyName = "mention_everyone")]
        internal bool MentionEveryone;
        [JsonProperty(PropertyName = "attachments")]
        internal MessageAttachment[] Attachment;
        [JsonProperty(PropertyName = "reactions")]
        internal Reaction[] Reactions;
        [JsonProperty(PropertyName = "nonce")]
        internal string Nonce;
        [JsonProperty(PropertyName = "pinned")]
        internal bool Pinned;
        [JsonProperty(PropertyName = "webhook_id")] //TODO : если будет вебхук объект - вставить сюда
        internal string WebhookIdentifier;
        [JsonProperty(PropertyName = "application")]
        internal MessageApplication Application;
        [JsonProperty(PropertyName = "message_reference")]
        internal MessageReference MessageReference;

        internal IReadOnlyCollection<IChannel> MentionedChannels { get; private set; }
        internal IReadOnlyCollection<Role> MentionedRoles { get; private set; }
        internal IReadOnlyCollection<IUser> MentionedUsers { get; private set; }
        internal IUser Author { get; private set; } //TODO : все поля завершить
        internal bool IsWebhook => WebhookIdentifier != null;

        [JsonProperty(PropertyName = "mention_channels")]
        private ChannelMention[] mentionedChannels = new ChannelMention[0]; //Inicializate this fields with stub
        [JsonProperty(PropertyName = "mention_roles")]                      //(empty array) is needed for proper
        private string[] mentionedRoles = new string[0];                    //foreach loop in CompleteDeserialization
        [JsonProperty(PropertyName = "mentions")]
        private User[] mentionedUsers = new User[0];
        [JsonProperty(PropertyName = "author")]
        private User author;

        [OnDeserialized]
        private void CompleteDeserialization(StreamingContext context)
        {
            List<Role> targetRoles = new List<Role>(capacity: mentionedRoles.Length);
            List<IUser> targetUsers = new List<IUser>(capacity: mentionedUsers.Length);
            List<IChannel> targetChannels = new List<IChannel>(capacity: mentionedChannels.Length);
            Guild targetGuild = DiscordGatewayClient.TryToGetGuild(guildIdentifier) as Guild;
            IUser targetAuthor = null;
            IChannel targetChannel = null;

            if (targetGuild != null)
            {
                targetAuthor = targetGuild.TryToGetUser(author.Identifier);
                targetChannel = targetGuild.TryToGetChannel(channelIdentifier);
            }
            foreach (string roleId in mentionedRoles)
            {
                Role role = targetGuild.TryToGetRole(roleId);
                if (role != null)
                    targetRoles.Add(role);
            }
            foreach(User user in mentionedUsers)
            {
                targetUsers.Add(targetGuild.TryToGetUser(user.Identifier));
            }
            foreach (ChannelMention channelMention in mentionedChannels)
            {
                IChannel channel = targetGuild.TryToGetChannel(channelMention.Identifier);
                if (channel != null)
                    targetChannels.Add(channel);
            }
            Guild = targetGuild;
            Channel = targetChannel;
            MentionedRoles = targetRoles;
            MentionedUsers = targetUsers;
            MentionedChannels = targetChannels;
            Author = targetAuthor;
            if(MessageReference != null)
                UpdateMessageReferences();
        }
        private void UpdateMessageReferences()
        {
            MessageReference.Channel = Channel;
            MessageReference.Guild = Guild;
            MessageReference.Message = this;
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class ChannelMention
        {
            [JsonProperty(PropertyName = "id")]
            internal string Identifier;
            [JsonProperty(PropertyName = "guild_id")]
            internal string GuildId;
            [JsonProperty(PropertyName = "type")]
            internal ChannelType Type;
            [JsonProperty(PropertyName = "name")]
            internal string Name;
        }
    }

    internal class MessageActivity
    {
        [JsonProperty(PropertyName = "type")]
        internal MessageActivityType Type;
        [JsonProperty(PropertyName = "party_id")]
        internal string PartyId;
    }

    internal class MessageAttachment //TAI : хранить адресс как Uri, а не как строку
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
        [JsonProperty(PropertyName = "height")]
        internal int Height; //Only if file == image
        [JsonProperty(PropertyName = "width")]
        internal int Width;
    }

    internal class MessageApplication
    {
        [JsonProperty(PropertyName = "id")]
        internal string Identifier;
        [JsonProperty(PropertyName = "cover_image")]
        internal string CoverImage;
        [JsonProperty(PropertyName = "description")]
        internal string Description;
        [JsonProperty(PropertyName = "icon")]
        internal string Icon;
        [JsonProperty(PropertyName = "name")]
        internal string Name;
    }

    internal class MessageReference
    {
        internal Message Message;
        internal IChannel Channel;
        internal IGuild Guild;

        [JsonProperty(PropertyName = "message_id")]
        private string messageIdentifier;
        [JsonProperty(PropertyName = "channel_id")]
        private string channelIdentifier;
        [JsonProperty(PropertyName = "guild_id")]
        private string guildIdentifier;
    }

    internal class Reaction
    {
        [JsonProperty(PropertyName = "count")]
        internal int Count;
        [JsonProperty(PropertyName = "me")]
        internal bool Me;
        [JsonProperty(PropertyName = "emoji")]
        internal Emoji Emoji; // TODO : необходимо понять, что есть partiel-emoji и мб созлдать новый класс
    }

    [Flags]
    internal enum MessageFlags : int
    {
        Crossposted          = 1<<0,
        IsCrossposted        = 1<<1,
        SuppressEmbeds       = 1<<2,
        SourceMessageDeleted = 1<<3,
        Urgent               = 1<<4
    }
    internal enum MessageType : byte
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
    internal enum MessageActivityType : byte
    {
        Join,
        Spectrate,
        Listen,
        JoinRequest = 5
    }
}