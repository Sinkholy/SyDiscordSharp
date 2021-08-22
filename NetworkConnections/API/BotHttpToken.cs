namespace Http
{
	public class BotHttpToken
	{
		public BotHttpToken(string type, string value)
		{
			Type = type;
			Value = value;
		}

		public string Type { get; private set; }
		public string Value { get; private set; }
	}
}
