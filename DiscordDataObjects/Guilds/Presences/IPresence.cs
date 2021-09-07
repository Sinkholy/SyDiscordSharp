using DiscordDataObjects.Users.Activities;

using Gateway.Payload.DataObjects.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DiscordDataObjects.Guilds.Presences
{
    [JsonConverter(typeof(IPresenceConverter))]
    public interface IPresence
    {
        string UserIdentifier { get; }
        string GuildIdentifier { get; }
        IReadOnlyCollection<string> UserRolesIdentifier { get; }
        DateTime? PremiumSince { get; }
        UserStatus UserStatus { get; }
        ClientPlatformStatuses UserPlatformStatuses { get; }
        string Nickname { get; }
        IActivity VisibleActivity { get; }
        IReadOnlyCollection<IActivity> Activities { get; }
    }
    internal class IPresenceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IPresence);
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Presence result = new Presence();
            JsonConvert.PopulateObject(JObject.Load(reader).ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
