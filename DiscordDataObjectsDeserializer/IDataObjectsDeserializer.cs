namespace DiscordDataObjectsDeserializer
{
	public interface IDataObjectsDeserializer<TSerialized>
	{
		ConversionResult<T> Deserialize<T>(TSerialized serialized);
	}
}
