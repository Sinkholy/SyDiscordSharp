namespace Http
{
	public class AuthenticationResult
	{
		internal AuthenticationResult(bool isSuccessfull, string faultReason)
		{
			IsSuccessfull = isSuccessfull;
			FaultReason = faultReason;
		}

		public bool IsSuccessfull { get; private set; }
		public string FaultReason { get; private set; }
	}
}
