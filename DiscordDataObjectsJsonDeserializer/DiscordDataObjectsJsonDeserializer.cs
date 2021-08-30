using System;
using System.Collections.Generic;

using DiscordDataObjectsDeserializer;

using Newtonsoft.Json;

namespace DiscordDataObjectsJsonDeserializer
{
	public class DiscordDataObjectsJsonDeserializer : IDiscordDataObjectsStringDeserializer // TODO: подумай над тем, чтобы использовать JsonSerializer
	{
		readonly Dictionary<Type, JsonConverter[]> convertersByType;

		public DiscordDataObjectsJsonDeserializer()
		{
			convertersByType = new Dictionary<Type, JsonConverter[]>();
		}

		public DeserializationResult<T> Deserialize<T>(string value)
		{
			var error = DeserializationError.None;
			var errorDesc = string.Empty;
			T deserialized = default;
			bool typeCanBeConverted = IsTypePresented(typeof(T));
			if (typeCanBeConverted)
			{
				JsonConverter[] converters = GetTypeConverters(typeof(T));
				deserialized = JsonConvert.DeserializeObject<T>(value, converters);
			}
			else
			{
				error = DeserializationError.UnknownType;
				errorDesc = "Type cannot be converted. No match converters"; // TODO: более подробно описать ошибку
			}
			return new DeserializationResult<T>(deserialized, error, errorDesc);
		}
		public void AddNewMultipleTypeConverters(Type targetType, JsonConverter[] converters)
		{
			if (targetType is null)
			{
				throw new ArgumentNullException("targetType");
			}
			if(converters is null)
			{
				throw new ArgumentNullException("converters");
			}
			if (converters.Length == 0)
			{
				string exceptionDesc = "Cannot add type without converters.";
				throw new ArgumentException(exceptionDesc, "converters");
			}
			for (int i = 0; i < converters.Length; i++)
			{
				if (converters[i] is null)
				{
					string exceptionDesc = $"{converters[i]} was null. Cannot add null converter as type converter.";
					throw new ArgumentException(exceptionDesc, "converters");
				}
			}
			if (!IsTypePresented(targetType))
			{
				AddNewDeserializableType(targetType);
			}
			SetTypeConverters(targetType, converters);
		}
		public void AddNewTypeConverter(Type targetType, JsonConverter converter)
		{
			if (targetType is null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (converter is null)
			{
				throw new ArgumentNullException("converter");
			}
			if (!IsTypePresented(targetType))
			{
				AddNewDeserializableType(targetType);
			}
			SetTypeConverters(targetType, new JsonConverter[] { converter });
		}
		void AddNewDeserializableType(Type type)
		{
			convertersByType.Add(type, null);
		}
		bool IsTypePresented(Type targetType)
		{
			return convertersByType.ContainsKey(targetType);
		}
		JsonConverter[] GetTypeConverters(Type targetType)
		{
			return convertersByType[targetType];
		}
		void SetTypeConverters(Type targetType, JsonConverter[] converters)
		{
			convertersByType[targetType] = converters;
		}
	}
}
