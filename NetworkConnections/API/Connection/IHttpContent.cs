using System.IO;
using System.Threading.Tasks;

namespace HttpCommunication.Connection
{
	public interface IHttpContent
	{
		Task<string> ReadAsStringAsync();
		Task<byte[]> ReadAsByteArrayAsync();
		Task<Stream> ReadAsStreamAsyncAsync();
	}
}
