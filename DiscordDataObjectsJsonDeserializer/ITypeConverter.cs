using DiscordDataObjectsDeserializer;

namespace DiscordDataObjectsJsonDeserializer
{
	public interface ITypeConverter<TSerialized, TDeserialized>
	{
		ConversionResult<TDeserialized> Deserialize(TSerialized serialized);
		ConversionResult<TSerialized> Serialize(TDeserialized serializable);

		bool CanSerialize { get; }
		bool CanDeserialize { get; }
	}
}
