namespace DiscordDataObjectsDeserializer
{
	public class ConversionResult<T>
	{
		public static ConversionResult<T> Successfull(T result)
		{
			return new ConversionResult<T>() { Result = result, Error = null};
		}
		public static ConversionResult<T> Failed(ConversionErrorEnum error, string errorDescription, ConversionError nestedError = null)
		{
			var err = new ConversionError(error, errorDescription, nestedError);
			return new ConversionResult<T>() { Result = default, Error = err };
		}

		public bool IsSuccessfull => Error == null;
		public ConversionError Error { get; private set; }
		public T Result { get; private set; }
	}
	public class ConversionError
	{
		internal ConversionError(ConversionErrorEnum error, string errorDescription, ConversionError nestedError)
		{
			Error = error;
			ErrorDescription = errorDescription;
			NestedError = nestedError;
		}

		public ConversionErrorEnum Error { get; private set; }
		public string ErrorDescription { get; private set; }
		public ConversionError NestedError { get; private set; }
	}
	public enum ConversionErrorEnum : byte
	{
		None,
		UnknownType,
		InputDataError
	}
}
