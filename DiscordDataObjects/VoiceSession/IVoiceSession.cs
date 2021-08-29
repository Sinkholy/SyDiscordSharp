using Gateway.DataObjects.Voice;
using Gateway.Entities.Users;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway.Entities.VoiceSession
{
    [JsonConverter(typeof(IVoiceSessionConverter))]
    public interface IVoiceSession
    {
        string UserIdentifier { get; }
        IGuildUser User { get; }
        string ChannelIdentifier { get; }
        string GuildIdentifier { get; }
        string SessionIdentifier { get; }
        bool Deafened { get; }
        bool Muted { get; }        
        bool SelfDeafened { get; }
        bool SelfMuted { get; }
        bool SelfStream { get; }
        bool SelfVideo { get; }
        bool Suppressed { get; }
        DateTime RequestToSpeakTimestamp { get; }
    }

    internal class IVoiceSessionConverter : JsonConverter // TODO: вытащить все конвертеры куда-нибудь
                                                          // и поместить атрибуты уже в самом парсере
                                                          // а не над классами.
                                                          // Это позволит в будущем, при добавлении каких-либо
                                                          // дочерних классов просто менять аттрибут у непостребственно
                                                          // места где парсится объект, не влезая в изменения 
                                                          // этого класса\интерфейса
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IVoiceSession);
        }
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            VoiceSession result = new VoiceSession();
            JsonConvert.PopulateObject(json.ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
