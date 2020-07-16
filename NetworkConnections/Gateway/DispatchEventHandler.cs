using Gateway.DataObjects;
using Gateway.DataObjects.Voice;
using Gateway.Entities;
using Gateway.Entities.Guilds;
using Gateway.Entities.Invite;
using Gateway.Entities.Message;
using Gateway.Entities.Users;
using Gateway.Payload.DataObjects.Dispatch.DispatchEvents;
using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
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
        internal event EventHandler<EventHandlerArgs> MessageReactionEmoji = delegate { };
        internal event EventHandler<EventHandlerArgs> TypingStarted = delegate { };
        internal event EventHandler<EventHandlerArgs> UserUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> VoiceServerUpdated = delegate { };
        internal event EventHandler<EventHandlerArgs> WebhooksUpdated = delegate { };
        #endregion
        internal void OnNewEventCreated(string eventData, string eventName)
        {
            Enum.TryParse(eventName, out Events eventToRaiseName);
            if (eventToRaiseName == Events.UnknownEvent) 
                throw new Exception("UnknownEvent"); //TODO : здесь я не уверен, что нужно исключение, скорее логирование
            EventHandlerArgs eventArgs;
            object eventArgsData;
            switch (eventToRaiseName)
            {
                case Events.Ready:
                    eventArgsData = JsonConvert.DeserializeObject<Ready>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    Ready(this, eventArgs);
                    break;
                case Events.Resumed:
                    //TODO
                    break;
                case Events.Reconnect:
                    //TODO
                    break;
                case Events.ChannelCreated:
                    eventArgsData = JsonConvert.DeserializeObject<IChannel>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    ChannelCreated(this, eventArgs);
                    break;
                case Events.ChannelUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<IChannel>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    ChannelUpdated(this, eventArgs);
                    break;
                case Events.ChannelDeleted:
                    eventArgsData = JsonConvert.DeserializeObject<IChannel>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    ChannelDeleted(this, eventArgs);
                    break;
                case Events.ChannelPinsUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<ChannelPinsUpdatedEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    ChannelPinsUpdated(this, eventArgs);
                    break;
                case Events.GuildCreated:
                    eventArgsData = JsonConvert.DeserializeObject<Guild>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildCreated(this, eventArgs);
                    break;
                case Events.GuildUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<Guild>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildUpdated(this, eventArgs);
                    break;
                case Events.GuildDeleted:
                    eventArgsData = JsonConvert.DeserializeObject<GuildPreview>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildDeleted(this, eventArgs);
                    break;
                case Events.GuildBanAdded:
                    eventArgsData = JsonConvert.DeserializeObject<Ban>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildBanAdded(this, eventArgs);
                    break;
                case Events.GuildBanDeleted:
                    eventArgsData = JsonConvert.DeserializeObject<Ban>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildBanRemoved(this, eventArgs);
                    break;
                case Events.GuildEmojisUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<GuildEmojiUpdatedEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildEmojisUpdated(this, eventArgs);
                    break;
                case Events.GuildIntegrationUpdated: //TODO : хз что за интеграция нужно помять
                    //eventArgsData = JToken.Load(eventData);
                    //eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    //GuildIntegrationsUpdate(this, eventArgs);
                    break;
                case Events.GuildMemberAdded:
                    eventArgsData = JsonConvert.DeserializeObject<GuildUser>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildMemberAdded(this, eventArgs);
                    break;
                case Events.GuildMemberRemoved:
                    eventArgsData = JsonConvert.DeserializeObject<GuildMember>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildMemberRemoved(this, eventArgs);
                    break;
                case Events.GuildMemberUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<GuildUser>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildMemberUpdated(this, eventArgs);
                    break;
                case Events.GuildMembersChunkReceived:
                    eventArgsData = JsonConvert.DeserializeObject<GuildMemberChunk>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildMemberChunksReceived(this, eventArgs);
                    break;
                case Events.GuildRoleCreated:
                    eventArgsData = JsonConvert.DeserializeObject<RoleEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildRoleCreated(this, eventArgs);
                    break;
                case Events.GuildRoleUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<RoleEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildRoleUpdated(this, eventArgs);
                    break;
                case Events.GuildRoleDeleted:
                    eventArgsData = JsonConvert.DeserializeObject<RoleEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    GuildRoleDeleted(this, eventArgs);
                    break;
                case Events.InviteCreated:
                    eventArgsData = JsonConvert.DeserializeObject<Invite>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    InviteCreated(this, eventArgs);
                    break;
                case Events.InviteDeleted:
                    eventArgsData = JsonConvert.DeserializeObject<InviteBase>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    InviteDeleted(this, eventArgs);
                    break;
                case Events.MessageCreated:
                    eventArgsData = JsonConvert.DeserializeObject<Message>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageCreated(this, eventArgs);
                    break;
                case Events.MessageUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<Message>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageUpdated(this, eventArgs);
                    break;
                case Events.MessageDeleted:
                    eventArgsData = JsonConvert.DeserializeObject<MessageBase>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageDeleted(this, eventArgs);
                    break;
                case Events.MessageDeletedBulk:
                    eventArgsData = JsonConvert.DeserializeObject<MessageBase[]>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageDeletedBulk(this, eventArgs);
                    break;
                case Events.MessageReactionAdded:
                    eventArgsData = JsonConvert.DeserializeObject<MessageReactionEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageReactionAdded(this, eventArgs);
                    break;
                case Events.MessageReactionRemoved:
                    eventArgsData = JsonConvert.DeserializeObject<MessageReactionEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageReactionRemoved(this, eventArgs);
                    break;
                case Events.MessageReactionRemovedAll:
                    eventArgsData = JsonConvert.DeserializeObject<MessageReactionEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageReactionRemovedAll(this, eventArgs);
                    break;
                case Events.MessageReactionEmojiRemoved:
                    eventArgsData = JsonConvert.DeserializeObject<MessageReactionEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    MessageReactionEmoji(this, eventArgs);
                    break;
                case Events.PresenceUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<PresenceUpdatedEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    PresenceUpdated(this, eventArgs);
                    break;
                case Events.TypingStarted:
                    eventArgsData = JsonConvert.DeserializeObject<UserTypingEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    TypingStarted(this, eventArgs);
                    break;
                case Events.UserUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<User>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    UserUpdated(this, eventArgs);
                    break;
                case Events.VoiceStateUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<VoiceState>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    VoiceStateUpdated(this, eventArgs);
                    break;
                case Events.VoiceServerUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<VoiceServerUpdate>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    VoiceServerUpdated(this, eventArgs);
                    break;
                case Events.WebhooksUpdated:
                    eventArgsData = JsonConvert.DeserializeObject<WebhookUpdatedEvent>(eventData);
                    eventArgs = new EventHandlerArgs(eventArgsData.GetType(), eventToRaiseName, eventArgsData);
                    WebhooksUpdated(this, eventArgs);
                    break;
            }
        }
        #region Ctor's
        internal DispatchEventHandler() { }
        #endregion
        internal class EventHandlerArgs : EventArgs
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
}