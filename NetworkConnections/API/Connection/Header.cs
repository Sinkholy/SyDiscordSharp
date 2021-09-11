namespace Http.Connection
{
	public class Header
	{
		public Header(string scheme, string parameter)
		{
			Scheme = scheme;
			Parameter = parameter;
		}

		public string Scheme { get; private set; }
		public string Parameter { get; private set; }
	}
}
