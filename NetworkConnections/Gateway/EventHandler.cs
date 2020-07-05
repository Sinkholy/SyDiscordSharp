using Gateway.DataObjects.Guilds;
using Gateway.Payload.Enums;
using Gateway.Payload.EventObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gateway
{
    internal class EventHandler
    {
        private readonly EventInfo[] events;
        private readonly Dictionary<Events, bool> handleEvent;
        internal delegate void newEvent(IEvent eventData);

        #region Events to raise
        internal event newEvent Ready = delegate { };
        internal event newEvent GuildCreate = delegate { };
        internal event newEvent GuildUpdate = delegate { };
        internal event newEvent GuildDelete = delegate { };
        internal event newEvent GuildRoleCreate = delegate { };
        internal event newEvent GuildRoleUpdate = delegate { };
        internal event newEvent GuildRoleDelete = delegate { };
        internal event newEvent ChannelCreate = delegate { };
        internal event newEvent ChannelUpdate = delegate { };
        internal event newEvent ChannelDelete = delegate { };
        internal event newEvent ChannelPinsUpdate = delegate { };
        internal event newEvent GuildMemberAdd = delegate { };
        internal event newEvent GuildMemberUpdate = delegate { };
        internal event newEvent GuildMemberRemove = delegate { };
        internal event newEvent GuildBanAdd = delegate { };
        internal event newEvent GuildBanRemove = delegate { };
        internal event newEvent GuildEmojisUpdate = delegate { };
        internal event newEvent GuildIntegrationsUpdate = delegate { };
        internal event newEvent WebhooksUpdate = delegate { };
        internal event newEvent InviteCreate = delegate { };
        internal event newEvent InviteDelete = delegate { };
        internal event newEvent VoiceStateUpdate = delegate { };
        internal event newEvent PresenceUpdate = delegate { };
        internal event newEvent MessageCreate = delegate { };
        internal event newEvent MessageUpdate = delegate { };
        internal event newEvent MessageDelete = delegate { };
        internal event newEvent MessageDeleteBulk = delegate { };
        internal event newEvent MessageReactionAdd = delegate { };
        internal event newEvent MessageReactionRemove = delegate { };
        internal event newEvent MessageReactionRemoveAll = delegate { };
        internal event newEvent MessageReactionEmoji = delegate { };
        internal event newEvent TypingStart = delegate { };
        #endregion
        internal void OnNewEventCreated(string eventData, string eventName)
        {
            Enum.TryParse(eventName, out Events currentEventName);
            if (currentEventName == Events.UnknownEvent) throw new Exception("UnknownEvent"); //TODO : здесь я не уверен, что нужно исключение, скорее логирование
            IEvent currentEventData = IEventFactory(eventData, currentEventName);
            switch (currentEventName)
            {
                case Events.CHANNEL_CREATE:
                    ChannelCreate(currentEventData);
                    break;
                case Events.CHANNEL_DELETE:
                    ChannelDelete(currentEventData);
                    break;
                case Events.CHANNEL_PINS_UPDATE:
                    ChannelPinsUpdate(currentEventData);
                    break;
                case Events.CHANNEL_UPDATE:
                    ChannelUpdate(currentEventData);
                    break;
                case Events.GUILD_BAN_ADD:
                    GuildBanAdd(currentEventData);
                    break;
                case Events.GUILD_BAN_REMOVE:
                    GuildBanRemove(currentEventData);
                    break;
                case Events.GUILD_CREATE:
                    GuildCreate(currentEventData);
                    break;
                case Events.GUILD_DELETE:
                    GuildDelete(currentEventData);
                    break;
                case Events.GUILD_EMOJIS_UPDATE:
                    GuildEmojisUpdate(currentEventData);
                    break;
                case Events.GUILD_INTEGRATIONS_UPDATE:
                    GuildIntegrationsUpdate(currentEventData);
                    break;
                case Events.GUILD_MEMBER_ADD:
                    GuildMemberAdd(currentEventData);
                    break;
                case Events.GUILD_MEMBER_REMOVE:
                    GuildMemberRemove(currentEventData);
                    break;
                case Events.GUILD_MEMBER_UPDATE:
                    GuildMemberUpdate(currentEventData);
                    break;
                case Events.GUILD_ROLE_CREATE:
                    GuildRoleCreate(currentEventData);
                    break;
                case Events.GUILD_ROLE_DELETE:
                    GuildRoleDelete(currentEventData);
                    break;
                case Events.GUILD_ROLE_UPDATE:
                    GuildRoleUpdate(currentEventData);
                    break;
                case Events.GUILD_UPDATE:
                    GuildUpdate(currentEventData);
                    break;
                case Events.INVITE_CREATE:
                    InviteCreate(currentEventData);
                    break;
                case Events.INVITE_DELETE:
                    InviteDelete(currentEventData);
                    break;
                case Events.MESSAGE_CREATE:
                    MessageCreate(currentEventData);
                    break;
                case Events.MESSAGE_DELETE:
                    MessageDelete(currentEventData);
                    break;
                case Events.MESSAGE_DELETE_BULK:
                    MessageDeleteBulk(currentEventData);
                    break;
                case Events.MESSAGE_REACTION_ADD:
                    MessageReactionAdd(currentEventData);
                    break;
                case Events.MESSAGE_REACTION_REMOVE:
                    MessageReactionRemove(currentEventData);
                    break;
                case Events.MESSAGE_REACTION_REMOVE_ALL:
                    MessageReactionRemoveAll(currentEventData);
                    break;
                case Events.MESSAGE_REACTION_REMOVE_EMOJI:
                    MessageReactionEmoji(currentEventData);
                    break;
                case Events.MESSAGE_UPDATE:
                    MessageUpdate(currentEventData);
                    break;
                case Events.PRESENCE_UPDATE:
                    PresenceUpdate(currentEventData);
                    break;
                case Events.READY:
                    Ready(currentEventData);
                    break;
                case Events.TYPING_START:
                    TypingStart(currentEventData);
                    break;
                case Events.VOICE_STATE_UPDATE:
                    VoiceStateUpdate(currentEventData);
                    break;
                case Events.WEBHOOKS_UPDATE:
                    WebhooksUpdate(currentEventData);
                    break;
                default:
                    throw new Exception("Unknown event"); //TODO : АААААААААА ИСКЛЮЧЕНИЕ
            }
        }
        private IEvent IEventFactory(string eventData, Events eventName)
        {
            Guild guild;
            if (eventName == Events.READY) return JsonConvert.DeserializeObject<Ready>(eventData);
            else if (eventName == Events.GUILD_CREATE)
            {
                guild = JsonConvert.DeserializeObject<Guild>(eventData);
                return guild;
            }
            else if (eventName == Events.MESSAGE_REACTION_ADD)
            {

            }
            return new SomeEvent();
        }
        internal EventHandler()
        {
            events = GetType().GetEvents(BindingFlags.NonPublic | BindingFlags.Instance);
            handleEvent = new Dictionary<Events, bool>(capacity: events.Length);
        }
        private class SomeEvent : IEvent { }
    }
}