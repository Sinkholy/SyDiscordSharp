namespace DiscordDataObjectsDeserializer
{
	public class DeserializationResult<T>
	{
		public DeserializationResult(T result, 
									DeserializationError error = DeserializationError.None,
									string errorDescription = null)
		{
			Error = error;
			ErrorDescription = errorDescription;
			Result = result;
		}

		public bool IsSuccessfull => Error == DeserializationError.None;
		public DeserializationError Error { get; private set; }
		public string ErrorDescription { get; private set; }
		public T Result { get; private set; }
	}
	public enum DeserializationError : byte
	{
		None,
		UnknownType,
		InputDataError
	}
}
