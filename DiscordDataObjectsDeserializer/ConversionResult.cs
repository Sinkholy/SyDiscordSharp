namespace DiscordDataObjectsDeserializer
{
	public class ConversionResult<T>
	{
		public static ConversionResult<T> Successfull(T result)
		{
			return new ConversionResult<T>() { Result = result, Error = ConversionError.None, ErrorDescription = string.Empty};
		}
		public static ConversionResult<T> Failed(ConversionError error, string errorDescription)
		{
			return new ConversionResult<T>() { Result = default, Error = error, ErrorDescription = errorDescription };
		}

		public bool IsSuccessfull => Error == ConversionError.None;
		public ConversionError Error { get; private set; }
		public string ErrorDescription { get; private set; }
		public T Result { get; private set; }
	}
	public enum ConversionError : byte
	{
		None,
		UnknownType,
		InputDataError
	}
}
