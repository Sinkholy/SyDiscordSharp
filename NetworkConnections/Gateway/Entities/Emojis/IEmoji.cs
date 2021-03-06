﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


namespace Gateway.Entities.Emojis
{
    [JsonConverter(typeof(IEmojiConverter))]
    public interface IEmoji
    {
        string Identifier { get; }
        string Name { get; }
        bool IsAnimated { get; }
        bool IsUnicodeEmoji { get; }
        string Mention { get; }
        string UrlEncoded { get; }
    }

    internal class IEmojiConverter : JsonConverter // TODO: вытащить все конвертеры куда-нибудь
                                                   // и поместить атрибуты уже в самом парсере
                                                   // а не над классами.
                                                   // Это позволит в будущем, при добавлении каких-либо
                                                   // дочерних классов просто менять аттрибут у непостребственно
                                                   // места где парсится объект, не влезая в изменения 
                                                   // этого класса\интерфейса
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IEmoji);
        }
        public override bool CanRead => true;
        public override bool CanWrite => false; // TODO: исплементировать запись
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            IEmoji result;
            if (string.IsNullOrWhiteSpace(json["id"]?.ToString() ?? string.Empty))
            {
                result = new Emoji();
            }
            else
            {
                result = new GuildEmoji();
            }
            JsonConvert.PopulateObject(json.ToString(), result);
            return result;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
