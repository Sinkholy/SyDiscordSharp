using Newtonsoft.Json;
using System;

namespace DiscordDataObjects.Channels.Message.Embed
{
    /// <summary>
    /// Total embed characters count must not exceed 6000 characters 
    /// in total. Violating will result in a Bad Request response.
    /// </summary>
    public class EmbedData // TAI: интерфейс или я совсем уже куку? (пока что склоняюсь к отрицанию интерфейса)
    {
        /// <summary>
        /// Limit: 256 characters
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; private set; }
        /// <summary>
        /// Limit: 2048 characters
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; private set; }
        [JsonProperty(PropertyName = "url")]
        public string Uri { get; private set; }
        [JsonProperty(PropertyName = "color")]
        public int Color { get; private set; } // TAI: преобразовывать в hex?
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime? Timestamp { get; private set; }
        [JsonProperty(PropertyName = "footer")]
        public EmbedFooter Footer { get; private set; }
        [JsonProperty(PropertyName = "image")]
        public EmbedImage Image { get; private set; }
        [JsonProperty(PropertyName = "thumbnail")]
        public EmbedImage Thumbnail { get; private set; }
        [JsonProperty(PropertyName = "video")]
        public EmbedVideo Video { get; private set; }
        [JsonProperty(PropertyName = "author")]
        public EmbedAuthor Author { get; private set; }
        [JsonProperty(PropertyName = "provider")]
        public EmbedProvider Provider { get; private set; }
        /// <summary>
        /// Limit: 25 fields
        /// </summary>
        [JsonProperty(PropertyName = "fields")]
        public EmbedField[] Fields { get; private set; }

        public EmbedData(string title,
                         string description,
                         string uri,
                         int color,
                         DateTime? timestamp,
                         EmbedFooter footer = null,
                         EmbedImage image = null,
                         EmbedImage thumbnail = null,
                         EmbedVideo video = null,
                         EmbedAuthor author = null,
                         EmbedField[] fields = null)
        {
            Title = title;
            Description = description;
            Uri = uri;
            Color = color;
            Timestamp = timestamp;
            Footer = footer;
            Image = image;
            Thumbnail = thumbnail;
            Video = video;
            Author = author;
            Fields = fields;
        }
    }
}
