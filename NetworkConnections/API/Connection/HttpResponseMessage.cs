namespace HttpCommunication.Connection
{
	public class HttpResponseMessage
	{
		public bool IsSuccessStatusCode { private set; get; }
		public HttpStatusCode StatusCode { private set; get; }
		public string ReasonPhrase { private set; get; }
		public IHttpContent Content { private set; get; }
	}
}
