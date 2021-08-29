using System;
using System.Collections.Generic;

using DiscordDataObjectsDeserializer;

using Newtonsoft.Json;

namespace DiscordDataObjectsJsonDeserializer
{
	public class DiscordDataObjectsJsonDeserializer : IDiscordDataObjectsStringDeserializer
	{
		readonly Dictionary<Type, ICollection<JsonConverter>> convertersByType;

		public DiscordDataObjectsJsonDeserializer()
		{
			// Инициализировать переменную типа convertersByType;
		}

		public DeserializationResult<T> Deserialize<T>(string value)
		{
			// Объявить переменную типа DesrializationError и инициализировать её значением None
			// Объявить переменную result типа T и инициализировать её значением null
			// Объявить переменную errorDesc типа string и инициализировать её значением string.empty;
			// Объявить bool переменную typeCanBeConverter и инициализировать её результатом вызова TryToGetTypeConverters()
			// Если тип не может быть конвертирован
				// DeserializationError = TypeCannotBeConverter
				// Присвоить описание ошибки десериализации = Type cannot be converted no match converters. Unkown type
			// Так же
				// Получить конвертеры типа
				// Вызывать Вызвать JsonConvert.DeserializeObject и передать ему тип, json-объект и конвертеры
				// Присвоить result результат вызова JsonConvert.DeserializeObject
			// Сконструировать DeserializationResult
			// Вернуть результат
		}
		public void AddNewMultipleTypeConverters(Type targetType, JsonConverter[] converter)
		{
			// Объявить переменную типа List<JsonConverter> - convertersToAdd 
			// Если тип ещё не представлен
				// Добавить новый тип к возможно десериализуемым
				// Добавить все конвертеры из converters в convertersToAdd
			// так же
				// Для каждого конвертера в converters
					// Если данный тип конвертера ещё не присутствует у типа
						// Добавить конвертер в convertersToAdd
			// Для каждого конвертера в convertersToAdd
				// Добавить конвертер к конвертерам типа
		}
		public void AddNewTypeConverter(Type targetType, JsonConverter converter)
		{
			// Если тип уже представлен
				// Если такого конвертера ещё нет в конвертерах типа
					// Добавить конвертер в массив конвертеров этого типа
			// Так же
				// Добавить новый тип к возможно десериализуемым
				// Добавить к этому типу конвертер
		}
		void AddNewDeserializableType(Type type)
		{
			// Добавить новый тип в коллекцию
			// Иницилализировать пустую коллекцию конвертеров
		}
		void AddNewTypeConverter(Type targetType, JsonConverter converter)
		{
			// Получить коллекцию десериализаторов типа
			// Добавить в коллекцию новый десериализатор
		}
		bool IsTypePresented(Type targetType)
		{
			// Объявить bool переменную result и инициализировать её значением false;
			// Если в коллекции присутствует тип соответвующий targetType
			// Присвоить result значение true
			// Вернуть результат
		}
		bool IsConverterPresented(Type targetType, JsonConverter converter)
		{
			// Объявить bool переменную result и инициализировать её значением false;
			// Получить коллекцию десериализаторов типа
			// Если в коллекции присутствует тип соответвующий converter
				// Присвоить result значение true
			// Вернуть результат
		}
		ICollection<JsonConverter> GetTypeConverters(Type targetType)
		{
			// Вернуть коллекцию конвертеров
		}
	}
}
