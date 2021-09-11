using System;
using System.Collections.Generic;

using DiscordDataObjectsDeserializer;

namespace DiscordDataObjectsJsonDeserializer
{
	public class DiscordDataObjectsJsonConverter : IDataObjectsDeserializer<string>, IDataObjectsSerializer<string>
	{
		const string NonDeserializableConverterError = "Convertion error. Presented type converter cannot deserialize.";
		const string NonSerializableConverterError = "Convertion error. Presented type converter cannot serialize.";
		const string NoTypeConverterError = "Convertion error. No converter for type presented.";

		readonly Dictionary<Type, object> converterByType;

		public DiscordDataObjectsJsonConverter()
		{
			converterByType = new Dictionary<Type, object>();
		}

		public ITypeConverter<string, object> DefaultSerializer { get; set; }
		bool DefaultSerializerPresented => DefaultSerializer != null;

		public bool IsTypeDeserializable<T>()
		{
			bool result = false;
			bool typeConverterExists = IsTypePresented<T>();
			if (typeConverterExists)
			{
				var converter = GetTypeConverter<T>();
				result = converter.CanDeserialize;
			}
			return result;
		}

		public bool IsTypeSerializable<T>()
		{
			bool result = false;
			bool typeConverterExists = IsTypePresented<T>();
			if (typeConverterExists)
			{
				var converter = GetTypeConverter<T>();
				result = converter.CanSerialize;
			}
			return result;
		}
		public ConversionResult<T> Deserialize<T>(string serialized)
		{
			bool typeCanBeConverted = IsTypePresented<T>();
			if (typeCanBeConverted)
			{
				var converter = GetTypeConverter<T>();
				return converter.CanDeserialize
					? converter.Deserialize(serialized)
					: ConversionResult<T>.Failed(ConversionErrorEnum.UnknownType, NonDeserializableConverterError);
			}
			else
			{
				return ConversionResult<T>.Failed(ConversionErrorEnum.UnknownType, NoTypeConverterError);
			}
		}
		public ConversionResult<string> Serialize<T>(T @object)
		{
			bool typeCanBeConverted = IsTypePresented<T>();
			if (typeCanBeConverted)
			{
				var converter = GetTypeConverter<T>();
				return converter.CanSerialize
					? converter.Serialize(@object)
					: ConversionResult<string>.Failed(ConversionErrorEnum.UnknownType, NonSerializableConverterError);
			}
			else
			{
				return DefaultSerializerPresented
					? DefaultSerializer.Serialize(@object)
					: ConversionResult<string>.Failed(ConversionErrorEnum.UnknownType, NoTypeConverterError);
			}
		}
		public void AddNewConvertableType<T>(ITypeConverter<string, T> converter)
		{
			if (converter is null)
			{
				throw new ArgumentNullException("converter");
			}
			if (IsTypePresented<T>())
			{
				string exceptionDescription = $"Converter for {typeof(T)} type already presented. Cannot add multiple converters.";
				throw new ArgumentException(exceptionDescription, "T");
			}
			AddNewConvertableTypeLocal<T>(converter);
		}
		bool IsTypePresented<T>()
		{
			return converterByType.ContainsKey(typeof(T));
		}
		void AddNewConvertableTypeLocal<T>(ITypeConverter<string, T> converter)
		{
			converterByType.Add(typeof(T), converter);
		}
		ITypeConverter<string, T> GetTypeConverter<T>()
		{
			return converterByType[typeof(T)] as ITypeConverter<string, T>;
		}
	}
}
