using DiscordDataObjects.Channels;
using DiscordDataObjects.Guilds;
using DiscordDataObjects.Guilds.Invite;
using DiscordDataObjects.Guilds.Presences;
using DiscordDataObjects.VoiceSession;

using Gateway.DataObjects;
using Gateway.DataObjects.Voice;
using Gateway.Payload.DataObjects.Dispatch.DispatchEvents;
using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gateway
{
    internal class DispatchEventHandler
    {
        #region Events to raise
        internal event EventHandler<EventHandlerArgs> Ready = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildCreated = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildDeleted = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildRoleCreated = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildRoleUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildRoleDeleted = delegate { };
        internal event EventHandler<EventHandlerArgs> ChannelCreated = delegate { };
        internal event EventHandler<EventHandlerArgs> ChannelUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> ChannelDeleted = delegate { };
        internal event EventHandler<EventHandlerArgs> ChannelPinsUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildMemberAdded = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildMemberUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildMemberRemoved = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildMemberChunksReceived = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildBanAdded = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildBanRemoved = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildEmojisUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> GuildIntegrationsUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> InviteCreated = delegate { };
        internal event EventHandler<EventHandlerArgs> InviteDeleted = delegate { };
        internal event EventHandler<EventHandlerArgs> VoiceStateUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> PresenceUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> MessageCreated = delegate { };
        internal event EventHandler<EventHandlerArgs> MessageUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> MessageDeleted = delegate { };
        internal event EventHandler<EventHandlerArgs> MessageDeletedBulk = delegate { };
        internal event EventHandler<EventHandlerArgs> MessageReactionAdded = delegate { };
        internal event EventHandler<EventHandlerArgs> MessageReactionRemoved = delegate { };
        internal event EventHandler<EventHandlerArgs> MessageReactionRemovedAll = delegate { };
        internal event EventHandler<EventHandlerArgs> MessageReactionEmojiRemoved = delegate { };
        internal event EventHandler<EventHandlerArgs> TypingStarted = delegate { };
        internal event EventHandler<EventHandlerArgs> UserUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> VoiceServerUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> WebhooksUpdated = delegate { };
        #endregion
        internal void OnNewClientEventReceived(string eventName, string eventData)
        {
            Enum.TryParse(eventName, out Events eventToRaiseName);
            if (eventToRaiseName == Events.UnknownEvent)
            {
                // TODO : интструмент логирования ($"Unknown dispatch event. /n Event name: {eventName} /n Event data: {eventData}");
            }
            EventHandlerArgs eventArgs;
            object eventArgsData;
            Console.WriteLine($"Dispatch: {eventName}");
            switch (eventToRaiseName)
            {
                case Events.READY:
                    eventArgsData = JsonConvert.DeserializeObject<Ready>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    Ready(this, eventArgs);
                    break;
                case Events.RESUMED:
                    Console.WriteLine("resumed");
                    //TODO
                    break;
                case Events.RECONNECT:
                    //TODO
                    Console.WriteLine("reconnected");
                    break;
                case Events.CHANNEL_CREATE:
                    eventArgsData = JsonConvert.DeserializeObject<IChannel>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    ChannelCreated(this, eventArgs);
                    break;
                case Events.CHANNEL_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<IChannel>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    ChannelUpdated(this, eventArgs);
                    break;
                case Events.CHANNEL_DELETE:
                    eventArgsData = JsonConvert.DeserializeObject<IChannel>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    ChannelDeleted(this, eventArgs);
                    break;
                case Events.CHANNEL_PINS_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<ChannelPinsUpdatedEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    ChannelPinsUpdated(this, eventArgs);
                    break;
                case Events.GUILD_CREATE:
                    eventArgsData = JsonConvert.DeserializeObject<Guild>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildCreated(this, eventArgs);
                    break;
                case Events.GUILD_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<Guild>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildUpdated(this, eventArgs);
                    break;
                case Events.GUILD_DELETE:
                    eventArgsData = JsonConvert.DeserializeObject<GuildPreview>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildDeleted(this, eventArgs);
                    break;
                case Events.GUILD_BAN_ADD:
                    eventArgsData = JsonConvert.DeserializeObject<Ban>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildBanAdded(this, eventArgs);
                    break;
                case Events.GUILD_BAN_REMOVE:
                    eventArgsData = JsonConvert.DeserializeObject<Ban>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildBanRemoved(this, eventArgs);
                    break;
                case Events.GUILD_EMOJIS_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<GuildEmojiUpdatedEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildEmojisUpdated(this, eventArgs);
                    break;
                case Events.GUILD_INTEGRATIONS_UPDATE: //TODO : хз что за интеграция нужно помять
                    //eventArgsData = JToken.Load(eventData);
                    //eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    //GuildIntegrationsUpdate(this, eventArgs);
                    break;
                case Events.GUILD_MEMBER_ADD:
                    eventArgsData = JsonConvert.DeserializeObject<GuildUser>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildMemberAdded(this, eventArgs);
                    break;
                case Events.GUILD_MEMBER_REMOVE:
                    eventArgsData = JsonConvert.DeserializeObject<GuildMember>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildMemberRemoved(this, eventArgs);
                    break;
                case Events.GUILD_MEMBER_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<GuildUser>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildMemberUpdated(this, eventArgs);
                    break;
                case Events.GuildMembersChunkReceived:
                    eventArgsData = JsonConvert.DeserializeObject<GuildMemberChunk>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildMemberChunksReceived(this, eventArgs);
                    break;
                case Events.GUILD_ROLE_CREATE:
                    eventArgsData = JsonConvert.DeserializeObject<RoleEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildRoleCreated(this, eventArgs);
                    break;
                case Events.GUILD_ROLE_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<RoleEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildRoleUpdated(this, eventArgs);
                    break;
                case Events.GUILD_ROLE_DELETE:
                    eventArgsData = JsonConvert.DeserializeObject<RoleDeletedEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildRoleDeleted(this, eventArgs);
                    break;
                case Events.INVITE_CREATE:
                    eventArgsData = JsonConvert.DeserializeObject<Invite>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    InviteCreated(this, eventArgs);
                    break;
                case Events.INVITE_DELETE:
                    eventArgsData = JsonConvert.DeserializeObject<InviteBase>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    InviteDeleted(this, eventArgs);
                    break;
                case Events.MESSAGE_CREATE:
                    eventArgsData = JsonConvert.DeserializeObject<Message>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageCreated(this, eventArgs);
                    break;
                case Events.MESSAGE_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<Message>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageUpdated(this, eventArgs);
                    break;
                case Events.MESSAGE_DELETE:
                    eventArgsData = JsonConvert.DeserializeObject<MessageBase>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageDeleted(this, eventArgs);
                    break;
                case Events.MESSAGE_DELETE_BULK:
                    eventArgsData = JsonConvert.DeserializeObject<MessageDeletedBulk>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageDeletedBulk(this, eventArgs);
                    break;
                case Events.MESSAGE_REACTION_ADD:
                    eventArgsData = JsonConvert.DeserializeObject<MessageReactionEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageReactionAdded(this, eventArgs);
                    break;
                case Events.MESSAGE_REACTION_REMOVE:
                    eventArgsData = JsonConvert.DeserializeObject<MessageReactionEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageReactionRemoved(this, eventArgs);
                    break;
                case Events.MESSAGE_REACTION_REMOVE_ALL:
                    eventArgsData = JsonConvert.DeserializeObject<MessageReactionEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageReactionRemovedAll(this, eventArgs);
                    break;
                case Events.MESSAGE_REACTION_REMOVE_EMOJI:
                    eventArgsData = JsonConvert.DeserializeObject<MessageReactionEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageReactionEmojiRemoved(this, eventArgs);
                    break;
                case Events.PRESENCE_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<IPresence>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    PresenceUpdated(this, eventArgs);
                    break;
                case Events.TYPING_START:
                    eventArgsData = JsonConvert.DeserializeObject<UserTypingEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    TypingStarted(this, eventArgs);
                    break;
                case Events.USER_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<User>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    UserUpdated(this, eventArgs);
                    break;
                case Events.VOICE_STATE_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<IVoiceSession>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    VoiceStateUpdated(this, eventArgs);
                    break;
                case Events.VOICE_SERVER_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<VoiceServerUpdate>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    VoiceServerUpdated(this, eventArgs);
                    break;
                case Events.WEBHOOKS_UPDATE:
                    eventArgsData = JsonConvert.DeserializeObject<WebhookUpdatedEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    WebhooksUpdated(this, eventArgs);
                    break;
            }
        }
        #region Ctor's
        internal DispatchEventHandler() { }
        #endregion
    }
    public class EventHandlerArgs : EventArgs
    {
        public Type EventDataObjectType { get; private set; }
        public Events EventName { get; private set; }
        public object EventData { get; private set; }
        internal EventHandlerArgs(Type eventDataType, Events eventName, object eventData)
        {
            EventDataObjectType = eventDataType;
            EventName = eventName;
            EventData = eventData;
        }
    }
}