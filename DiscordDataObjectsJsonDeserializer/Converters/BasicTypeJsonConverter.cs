using System;

using DiscordDataObjectsDeserializer;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordDataObjectsJsonDeserializer.Converters
{
    public abstract class BasicTypeJsonConverter<TDeserialized> : ITypeConverter<string, TDeserialized>
    {
        public abstract bool CanSerialize { get; }
        public abstract bool CanDeserialize { get; }

        public abstract ConversionResult<TDeserialized> Deserialize(string serialized);
        public abstract ConversionResult<string> Serialize(TDeserialized serializable);

        protected bool TryToParse(string serialized, out JObject result, out Exception exception)
        {
            bool isParsed = false;
            result = default;
            exception = default;
            try
            {
                result = JObject.Parse(serialized);
                isParsed = true;
            }
            catch (JsonReaderException ex)
            {
                exception = ex;
            }
            return isParsed;
        }
    }
}
