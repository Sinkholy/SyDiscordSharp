namespace Http.Connection
{
	public interface IHeaders
	{
		Header AuthorizationHeader { get; set; }
		Header UserAgentHeader { get; set; }
	}
}
