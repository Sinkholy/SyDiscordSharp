namespace DiscordDataObjectsDeserializer
{
	public interface IDataObjectsSerializer<TSerialized>
	{
		ConversionResult<TSerialized> Serialize<T>(T @object);
	}
}
