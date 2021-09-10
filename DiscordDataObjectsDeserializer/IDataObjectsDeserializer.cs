namespace DiscordDataObjectsDeserializer
{
	public interface IDataObjectsDeserializer<TSerialized>
	{
		ConversionResult<T> Deserialize<T>(TSerialized serialized);
		bool IsTypeDeserializable<T>();
	}
}
