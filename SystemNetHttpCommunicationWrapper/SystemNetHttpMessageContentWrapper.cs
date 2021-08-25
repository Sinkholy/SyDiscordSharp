using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using HttpCommunication.Connection;

namespace SystemNetHttpCommunicationWrapper
{
	partial class SystemNetHttpMessageContentWrapper 
	{
		readonly HttpContent httpContent;

		SystemNetHttpMessageContentWrapper(HttpContent content)
		{
			httpContent = content;
		}

		internal static SystemNetHttpMessageContentWrapper WrapFrom(HttpContent content)
		{
			return new SystemNetHttpMessageContentWrapper(content);
		}
	}

	partial class SystemNetHttpMessageContentWrapper : IHttpContent
	{
		public async Task<byte[]> ReadAsByteArrayAsync()
		{
			return await httpContent.ReadAsByteArrayAsync();
		}

		public async Task<Stream> ReadAsStreamAsyncAsync()
		{
			return await httpContent.ReadAsStreamAsync();
		}

		public async Task<string> ReadAsStringAsync()
		{
			return await httpContent.ReadAsStringAsync();
		}
	}
}
