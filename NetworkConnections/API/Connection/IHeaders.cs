namespace Http.Connection
{
	internal interface IHeaders
	{
		Header AuthorizationHeader { get; set; }
		Header UserAgentHeader { get; set; }
	}
}
