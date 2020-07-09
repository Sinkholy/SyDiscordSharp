﻿using Gateway.DataObjects.Channels;
using Gateway.DataObjects.Channels.Text;
using Gateway.DataObjects.Guilds;
using Gateway.DataObjects.Users;
using Gateway.Payload.EventObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
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
            DiscordGatewayClient gatewayClient = DiscordGatewayClient.GetInstance();
            List<Role> targetRoles = new List<Role>(capacity: mentionedRoles.Length);
            List<IUser> targetUsers = new List<IUser>(capacity: mentionedUsers.Length);
            List<IChannel> targetChannels = new List<IChannel>(capacity: mentionedChannels.Length);
            Guild targetGuild;
            IUser targetAuthor;
            IChannel targetChannel;

            if (gatewayClient.guilds[guildIdentifier] is Guild)
            {
                targetGuild = gatewayClient.guilds[guildIdentifier] as Guild;
                targetAuthor = targetGuild.Users.Where(x => x.Identifier == author.Identifier)
                                                .SingleOrDefault();
                targetChannel = targetGuild.Channels.Where(x => x.Identifier == channelIdentifier)
                                                    .SingleOrDefault();
            }
            else
            {
                throw new Exception();//TODO : исключение или зачем?
            }
            foreach (string roleId in mentionedRoles)
            {
                Role role = targetGuild.Roles.Where(x => x.Identifier == roleId).SingleOrDefault();
                if (role != null)
                    targetRoles.Add(role);
                else
                {
                    //TODO : исключенеи или куда?
                }
            }
            foreach(User user in mentionedUsers)
            {
                GuildUser guildUser = targetGuild.Users.Where(x => x.Identifier == user.Identifier).SingleOrDefault();
                if(guildUser != null)
                    targetUsers.Add(guildUser);
                else
                {
                    //TODO : исключение или почему?
                }
            }
            foreach (ChannelMention channelMention in mentionedChannels)
            {
                IChannel channel = 
                    targetGuild.Channels.Where(x => x.Identifier == channelMention.Identifier).SingleOrDefault();
                if (channel != null)
                    targetChannels.Add(channel);
                else
                {
                    //TODO : исключение или кому?
                }
            }
            Guild = targetGuild;
            Channel = targetChannel;
            MentionedRoles = targetRoles;
            MentionedUsers = targetUsers;
            MentionedChannels = targetChannels;
            Author = targetAuthor;
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

    [JsonObject(MemberSerialization.OptIn)]
    internal class MessageActivity
    {
        [JsonProperty(PropertyName = "type")]
        internal MessageActivityType Type;
        [JsonProperty(PropertyName = "party_id")]
        internal string PartyId;
    }

    [JsonObject(MemberSerialization.OptIn)]
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

    [JsonObject(MemberSerialization.OptIn)]
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

    [JsonObject(MemberSerialization.OptIn)]
    internal class MessageReference //TODO : доделать филды
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