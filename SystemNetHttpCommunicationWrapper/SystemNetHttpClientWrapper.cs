using System;
using System.Net.Http;
using System.Threading.Tasks;

using Http.Connection;

using HttpCommunication.Connection;

using OriginalRequestMessage = System.Net.Http.HttpRequestMessage;
using OriginalResponseMessage = System.Net.Http.HttpResponseMessage;
using WrappedRequestMessage = HttpCommunication.Connection.HttpRequestMessage;
using WrappedResponseMessage = HttpCommunication.Connection.HttpResponseMessage;

namespace SystemNetHttpCommunicationWrapper
{
	public class SystemNetHttpClientWrapper : IHttpConnection
	{
		readonly HttpMethod PatchMethod;
		readonly HttpClient httpClient;

		public SystemNetHttpClientWrapper()
		{
			httpClient = new HttpClient();
			BaseAdress = default;
			Headers = new HeadersWrapper(this);
			PatchMethod = new HttpMethod("PATCH");
		}

		public Uri BaseAdress
		{
			get => httpClient.BaseAddress;
			set => httpClient.BaseAddress = value;
		}
		public TimeSpan Timeout
		{
			get => httpClient.Timeout;
			set => httpClient.Timeout = value;
		}
		public IHeaders Headers { get; private set; }

		public async Task<WrappedResponseMessage> DeleteAsync(WrappedRequestMessage message)
		{
			OriginalResponseMessage response = await httpClient.DeleteAsync(message.DestinationEndpoint);
			WrappedResponseMessage result = ConvertReceivingMessage(response);
			return result;
		}
		public async Task<WrappedResponseMessage> GetAsync(WrappedRequestMessage message)
		{
			OriginalResponseMessage response = await httpClient.GetAsync(message.DestinationEndpoint);
			WrappedResponseMessage result = ConvertReceivingMessage(response);
			return result;
		}
		public async Task<WrappedResponseMessage> PatchAsync(WrappedRequestMessage message)
		{
			var requestMessage = new OriginalRequestMessage()
			{
				RequestUri = new Uri(message.DestinationEndpoint),
				Method = PatchMethod,
				Content = await ConvertSendingContent(message.Content)
			};
			OriginalResponseMessage response = await httpClient.SendAsync(requestMessage);
			WrappedResponseMessage result = ConvertReceivingMessage(response);
			return result;
		}
		public async Task<WrappedResponseMessage> PostAsync(WrappedRequestMessage message)
		{
			HttpContent contentToSend = await ConvertSendingContent(message.Content);
			OriginalResponseMessage response = await httpClient.PostAsync(message.DestinationEndpoint, contentToSend);
			WrappedResponseMessage result = ConvertReceivingMessage(response);
			return result;
		}
		public async Task<WrappedResponseMessage> PutAsync(WrappedRequestMessage message)
		{
			HttpContent contentToSend = await ConvertSendingContent(message.Content);
			OriginalResponseMessage response = await httpClient.PutAsync(message.DestinationEndpoint, contentToSend);
			WrappedResponseMessage result = ConvertReceivingMessage(response);
			return result;
		}

		async Task<HttpContent> ConvertSendingContent(IHttpContent toConvert)
		{
			var stringContent = await toConvert.ReadAsStringAsync();
			var convertedContent = new StringContent(stringContent);
			return convertedContent;
		}
		WrappedResponseMessage ConvertReceivingMessage(OriginalResponseMessage toConvert)
		{
			IHttpContent convertedContent = null;
			if(MessageContainsContent())
			{
				convertedContent = ConvertReceivingContent();
			}
			HttpStatusCode convertedStatusCode = ConvertSystemNetHttpStatusCode();
			var result = new WrappedResponseMessage(toConvert.IsSuccessStatusCode, convertedStatusCode, toConvert.ReasonPhrase, convertedContent);
			return result;

			bool MessageContainsContent()
			{
				return toConvert.Content != null;
			}
			IHttpContent ConvertReceivingContent()
			{
				return SystemNetHttpMessageContentWrapper.WrapFrom(toConvert.Content);
			}
			HttpStatusCode ConvertSystemNetHttpStatusCode() 
			{
				// UNDONE: здесь нужен Assert по поводу того, что имеется неизвестный код статуса
				// UNDONE: Так же нужна обработка исключения преобразования
				return (HttpStatusCode)toConvert.StatusCode;
			}
		}

		class HeadersWrapper : IHeaders
		{
			readonly SystemNetHttpClientWrapper client;

			internal HeadersWrapper(SystemNetHttpClientWrapper client)
			{
				this.client = client;
			}

			public Header AuthorizationHeader
			{
				get
				{
					var header = client.httpClient.DefaultRequestHeaders.Authorization;
					return new Header(header.Scheme, header.Parameter);
				}
				set
				{
					var header = new System.Net.Http.Headers.AuthenticationHeaderValue(value.Scheme, value.Parameter);
					client.httpClient.DefaultRequestHeaders.Authorization = header;
				}
			}
			public Header UserAgentHeader
			{
				get => client.Headers.UserAgentHeader;
				set => client.Headers.UserAgentHeader = value;
			}
		}
	}
}
