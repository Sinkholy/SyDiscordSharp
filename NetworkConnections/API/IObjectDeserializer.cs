namespace Http
{
	public interface IObjectDeserializer
	{
		T Deserialize<T>(string value);
	}
}
