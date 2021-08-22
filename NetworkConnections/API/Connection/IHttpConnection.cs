using System;
using System.Threading.Tasks;

using Http.Connection;

namespace HttpCommunication.Connection
{
	internal interface IHttpConnection
	{
		Uri BaseAdress { get; set; }
		TimeSpan Timeout { get; set; }
		IHeaders Headers { get; }

		Task<HttpResponseMessage> GetAsync(HttpRequestMessage message); 
		Task<HttpResponseMessage> PutAsync(HttpRequestMessage message); 
		Task<HttpResponseMessage> PostAsync(HttpRequestMessage message); 
		Task<HttpResponseMessage> PatchAsync(HttpRequestMessage message); 
		Task<HttpResponseMessage> DeleteAsync(HttpRequestMessage message); 
	}
}
