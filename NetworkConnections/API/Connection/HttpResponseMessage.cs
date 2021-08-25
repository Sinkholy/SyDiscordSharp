namespace HttpCommunication.Connection
{
	public class HttpResponseMessage
	{
		public HttpResponseMessage(bool isSuccessStatusCode, HttpStatusCode statusCode, string reasonPhrase, IHttpContent content)
		{
			IsSuccessStatusCode = isSuccessStatusCode;
			StatusCode = statusCode;
			ReasonPhrase = reasonPhrase;
			Content = content;
		}

		public bool IsSuccessStatusCode { private set; get; }
		public HttpStatusCode StatusCode { private set; get; }
		public string ReasonPhrase { private set; get; }
		public IHttpContent Content { private set; get; }
	}
}
