using System;
using System.Collections.Generic;

using DiscordDataObjectsDeserializer;

using Newtonsoft.Json;

namespace DiscordDataObjectsJsonDeserializer
{
	public class DiscordDataObjectsJsonConverter : IDataObjectsDeserializer<string>, IDataObjectsSerializer<string> 
	{
		// TODO: подумай над тем, чтобы использовать JsonSerializer
		readonly Dictionary<Type, JsonConverter> converterByType;

		public DiscordDataObjectsJsonConverter()
		{
			converterByType = new Dictionary<Type, JsonConverter>();
			SerializeIfNoConvertersPresented = false;
		}

		public bool SerializeIfNoConvertersPresented { get; private set; }

		public ConversionResult<T> Deserialize<T>(string serialized)
		{
			bool typeCanBeConverted = IsTypePresented(typeof(T));
			if (typeCanBeConverted)
			{
				// TODO: вероятнее всего необходимо обернуть непосредственно процесс конвертации
				// В блок try catch для того, чтобы более явно указывать тип ошибки.
				JsonConverter converter = GetTypeConverter(typeof(T));
				var deserialized = JsonConvert.DeserializeObject<T>(serialized, converter); 
				return ConversionResult<T>.Successfull(deserialized);
			}
			else
			{
				var error = ConversionError.UnknownType;
				string errorDesc = "Type cannot be deserialized. No match converters"; // TODO: более подробно описать ошибку
				return ConversionResult<T>.Failed(error, errorDesc);
			}
		}

		public ConversionResult<string> Serialize<T>(T @object)
		{
			// TODO: вероятнее всего необходимо обернуть непосредственно процесс конвертации
			// В блок try catch для того, чтобы более явно указывать тип ошибки если она появится.
			var targetType = typeof(T);
			if (IsTypePresented(targetType))
			{
				JsonConverter converter = GetTypeConverter(targetType);
				var serialized = JsonConvert.SerializeObject(@object, converter);
				return ConversionResult<string>.Successfull(serialized);
			}
			else if (SerializeIfNoConvertersPresented)
			{
				var serialized = JsonConvert.SerializeObject(@object);
				return ConversionResult<string>.Successfull(serialized);
			}
			else
			{
				var error = ConversionError.UnknownType;
				string errorDesc = "Type cannot be deserialized. No match converters"; // TODO: более подробно описать ошибку
				return ConversionResult<string>.Failed(error, errorDesc);
			}
		}
		public void AddNewConvertableType(Type targetType, JsonConverter converter)
		{
			if (targetType is null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (converter is null)
			{
				throw new ArgumentNullException("converter");
			}
			if (IsTypePresented(targetType))
			{
				string exceptionDescription = $"Converter for {targetType} type already presented. Cannot add multiple converters.";
				throw new ArgumentException(exceptionDescription, "targetType");
			}
			AddNewConvertableTypeLocal(targetType, converter);
		}
		bool IsTypePresented(Type targetType)
		{
			return converterByType.ContainsKey(targetType);
		}
		void AddNewConvertableTypeLocal(Type targetType, JsonConverter converter)
		{
			converterByType.Add(targetType, converter);
		}
		JsonConverter GetTypeConverter(Type targetType)
		{
			return converterByType[targetType];
		}
	}
}
