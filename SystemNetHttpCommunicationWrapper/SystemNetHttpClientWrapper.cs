using System;
using System.Net.Http;
using System.Threading.Tasks;

using Http.Connection;

using HttpCommunication.Connection;

using HttpResponseMessage = HttpCommunication.Connection.HttpResponseMessage;

namespace SystemNetHttpCommunicationWrapper
{
	class SystemNetHttpClientWrapper : IHttpConnection
	{
		const string PatchHttpMethod = "PATCH";
		readonly HttpClient httpClient;

		public SystemNetHttpClientWrapper()
		{
			// Инициализировать HttpClient
			// Инициализировать Timeout стандартным значением
			// Инициализировать BaseAddress стандартным значением
			// Инициализировать Headers используя HeadersWrapper класс
		}

		public Uri BaseAdress { get; set; }
		public TimeSpan Timeout { get; set; }
		public IHeaders Headers { get; private set; }

		public async Task<HttpResponseMessage> DeleteAsync(HttpRequestMessage message)
		{
			// Отправить запрос и дождаться его выполнения
			// Преобразовать полученный результат в HttpResponseMessage
			// Вернуть результат
		}
		public async Task<HttpResponseMessage> GetAsync(HttpRequestMessage message)
		{
			// Отправить запрос и дождаться его выполнения
			// Преобразовать полученное сообщения под IHttpResponseMessage интерфейс
			// Вернуть результат
		}
		public async Task<HttpResponseMessage> PatchAsync(HttpRequestMessage message)
		{
			// Подготовить HttpMethod в связи с тем, что Patch не входит в стандартный набор
			// Создать System.Net.Http.HttpRequestMessage
			// Отправить запрос и дождаться его выполнения
			// Преобразовать полученный результат в HttpResponseMessage
			// Вернуть результат
		}
		public async Task<HttpResponseMessage> PostAsync(HttpRequestMessage message)
		{
			// Преобразовать входной контент в HttpContent
			// Заполнить поле Uri взяв его из параметра Message
			// Отправить запрос и дождаться его выполнения
			// Преобразовать полученный результат в HttpResponseMessage
			// Вернуть результат
		}
		public async Task<HttpResponseMessage> PutAsync(HttpRequestMessage message)
		{
			// Преобразовать входной контент в HttpContent
			// Отправить запрос и дождаться его выполнения
			// Преобразовать полученный результат в HttpResponseMessage
			// Вернуть результат
		}
		public Header GetAuthorizationHeader()
		{
			// Перенаправить на вызов заголовка у хттп клиента
		}
		public void SetAuthorizationHeader()
		{
			// Перенаправить на вызов заголовка у хттп клиента
		}

		HttpContent ConvertSendingContent(IHttpContent toConvert)
		{
			// Прочитать контент из toConvert в строчном формате
			// Создать экземпляр типа StringContent 
			// Подставить в StringContent контент из toConvert
			// Вернуть результат
		}
		HttpResponseMessage ConvertReceivingMessage(HttpResponseMessage toConvert)
		{
			// Если в сообщении присутствует контент
				// Преобразовать полученный контент в IHttpContent (ConvertReceivingContent)
			// Преобразовать System.Net.HttpStatusCode в HttpStatusCode (ConvertSystemNetHttpStatusCode)
			// Создать HttpResponseMessage
			// Вернуть результат

			IHttpContent ConvertReceivingContent(HttpContent toConvert)
			{
				// Преобразовать
				// Вернуть результат
			}
			HttpStatusCode ConvertSystemNetHttpStatusCode(System.Net.HttpStatusCode toConvert)
			{
				// Проеобразовать
				// Вернуть результат
			}
		}

		class HeadersWrapper : IHeaders
		{
			readonly SystemNetHttpClientWrapper client;

			// Конструктор
			public Header AuthorizationHeader
			{
				get
				{
					// Перенаправить на вызов GetAuthorizeHeader у хттп клиента
				}
				set
				{
					// Перенаправить на вызов SetAuthorizeHeader у хттп клиента
				}
			}
			public Header UserAgentHeader
			{
				get
				{
					// Перенаправить на вызов GetUserAgent у хттп клиента
				}
				set
				{
					// Перенаправить на вызов SetUserAgent у хттп клиента
				}
			}
		}
	}
}
