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
        [Event(Events.READY)]
        internal event newEvent Ready = delegate { };
        [Event(Events.GUILD_CREATE)]
        internal event newEvent GuildCreate = delegate { };
        [Event(Events.GUILD_UPDATE)]
        internal event newEvent GuildUpdate = delegate { };
        [Event(Events.GUILD_DELETE)]
        internal event newEvent GuildDelete = delegate { };
        [Event(Events.GUILD_ROLE_CREATE)]
        internal event newEvent GuildRoleCreate = delegate { };
        [Event(Events.GUILD_ROLE_UPDATE)]
        internal event newEvent GuildRoleUpdate = delegate { };
        [Event(Events.GUILD_ROLE_DELETE)]
        internal event newEvent GuildRoleDelete = delegate { };
        [Event(Events.CHANNEL_CREATE)]
        internal event newEvent ChannelCreate = delegate { };
        [Event(Events.CHANNEL_UPDATE)]
        internal event newEvent ChannelUpdate = delegate { };
        [Event(Events.CHANNEL_DELETE)]
        internal event newEvent ChannelDelete = delegate { };
        [Event(Events.CHANNEL_PINS_UPDATE)]
        internal event newEvent ChannelPinsUpdate = delegate { };
        [Event(Events.GUILD_MEMBER_ADD)]
        internal event newEvent GuildMemberAdd = delegate { };
        [Event(Events.GUILD_MEMBER_UPDATE)]
        internal event newEvent GuildMemberUpdate = delegate { };
        [Event(Events.GUILD_MEMBER_REMOVE)]
        internal event newEvent GuildMemberRemove = delegate { };
        [Event(Events.GUILD_BAN_ADD)]
        internal event newEvent GuildBanAdd = delegate { };
        [Event(Events.GUILD_BAN_REMOVE)]
        internal event newEvent GuildBanRemove = delegate { };
        [Event(Events.GUILD_EMOJIS_UPDATE)]
        internal event newEvent GuildEmojisUpdate = delegate { };
        [Event(Events.GUILD_INTEGRATIONS_UPDATE)]
        internal event newEvent GuildIntegrationsUpdate = delegate { };
        [Event(Events.WEBHOOKS_UPDATE)]
        internal event newEvent WebhooksUpdate = delegate { };
        [Event(Events.INVITE_CREATE)]
        internal event newEvent InviteCreate = delegate { };
        [Event(Events.INVITE_DELETE)]
        internal event newEvent InviteDelete = delegate { };
        [Event(Events.VOICE_STATE_UPDATE)]
        internal event newEvent VoiceStateUpdate = delegate { };
        [Event(Events.PRESENCE_UPDATE)]
        internal event newEvent PresenceUpdate = delegate { };
        [Event(Events.MESSAGE_CREATE)]
        internal event newEvent MessageCreate = delegate { };
        [Event(Events.MESSAGE_UPDATE)]
        internal event newEvent MessageUpdate = delegate { };
        [Event(Events.MESSAGE_DELETE)]
        internal event newEvent MessageDelete = delegate { };
        [Event(Events.MESSAGE_DELETE_BULK)]
        internal event newEvent MessageDeleteBulk = delegate { };
        [Event(Events.MESSAGE_REACTION_ADD)]
        internal event newEvent MessageReactionAdd = delegate { };
        [Event(Events.MESSAGE_REACTION_REMOVE)]
        internal event newEvent MessageReactionRemove = delegate { };
        [Event(Events.MESSAGE_REACTION_REMOVE_ALL)]
        internal event newEvent MessageReactionRemoveAll = delegate { };
        [Event(Events.MESSAGE_REACTION_REMOVE_EMOJI)]
        internal event newEvent MessageReactionEmoji = delegate { };
        [Event(Events.TYPING_START)]
        internal event newEvent TypingStart = delegate { };
        #endregion
        internal void OnNewEventCreated(string eventData, string eventName)
        {
            Enum.TryParse(eventName, out Events currentEvent);
            if (currentEvent == Events.UnknownEvent) throw new Exception("UnknownEvent"); //TODO : здесь я не уверен, что нужно исключение, скорее логирование
            if (handleEvent.ContainsKey(currentEvent))
            {
                if (handleEvent[currentEvent])
                {
                    IEvent eventParsed = IEventFactory(eventData, currentEvent);
                    RaiseEvent(currentEvent, eventParsed);
                }
                else
                {
                    //TODO : логировать данные ивента для будущего восстановление или хотя бы ознакомления
                }
            }
            else
            {
                bool found = TryToGetEventMethod(currentEvent);
                if (found)
                {
                    IEvent eventParsed = IEventFactory(eventData, currentEvent);
                    RaiseEvent(currentEvent, eventParsed);
                }
                else
                {
                    //TODO : логировать данные ивента для будущего восстановление или хотя бы ознакомления
                }
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
        private bool TryToGetEventMethod(Events eventName)
        {
            Events currentIterationEventName;
            for (int i = 0; i < events.Length; i++)
            {
                currentIterationEventName = events[i].GetCustomAttribute<Event>().EventName;
                if (currentIterationEventName == eventName)
                {
                    handleEvent.Add(eventName, true);
                    return true;
                }
            }
            return false;
        }
        private string GetEventFieldName(Events eventName)
        {
            string result = string.Empty;
            Events currentIterationEventName;
            for (int i = 0; i < events.Length; i++)
            {
                currentIterationEventName = events[i].GetCustomAttribute<Event>().EventName;
                if (currentIterationEventName == eventName)
                {
                    result = events[i].Name;
                    break;
                }
            }
            return result;
        }
        internal EventHandler()
        {
            events = GetType().GetEvents(BindingFlags.NonPublic | BindingFlags.Instance);
            handleEvent = new Dictionary<Events, bool>(capacity: events.Length);
        }
        private Delegate[] GetEventDelegates(Events eventName)
        {
            string eventFieldName = GetEventFieldName(eventName);
            if (eventFieldName == string.Empty) throw new Exception("Cannot find event field"); //TODO : исключение или ну его нахер
            return (GetType().GetField(eventFieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this) as MulticastDelegate).GetInvocationList();
        }
        private void RaiseEvent(Events eventName, IEvent eventData)
        {
            object[] parameters = new object[1] { eventData };
            Delegate[] delegates = GetEventDelegates(eventName);
            foreach (var handler in delegates) Console.WriteLine(1); 
                //handler.Method.Invoke(handler.Target, parameters);
        }
        private class Event : Attribute
        {
            internal Events EventName { get; private set; }
            internal Event(Events eventName) => EventName = eventName;
        }
        private class SomeEvent : IEvent { }
    }
}