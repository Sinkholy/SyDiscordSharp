namespace Http.Connection
{
	public class Header
	{
		public Header(string scheme, string parameter)
		{
			Scheme = scheme;
			Parameter = parameter;
		}

		internal string Scheme { get; private set; }
		internal string Parameter { get; private set; }
	}
}
