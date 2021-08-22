namespace HttpCommunication.Connection
{
	public class HttpRequestMessage
	{
		public HttpRequestMessage(string destinationEndpoint, IHttpContent content)
		{
			DestinationEndpoint = destinationEndpoint;
			Content = content;
		}

		public string DestinationEndpoint { private set; get; }
		public IHttpContent Content { private set; get; }
	}
}
