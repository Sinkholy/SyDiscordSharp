namespace DiscordDataObjectsDeserializer
{
    public interface IDiscordDataObjectsStringDeserializer
    {
        DeserializationResult<T> Deserialize<T>(string value);
    }
}
