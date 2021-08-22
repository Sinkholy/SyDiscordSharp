using System;
using System.Threading.Tasks;

namespace HttpCommunication.Connection
{
	public interface IHttpConnection
	{
		Uri BaseAdress { get; set; }
		TimeSpan Timeout { get; set; }

		Task<HttpResponseMessage> GetAsync(HttpRequestMessage message); 
		Task<HttpResponseMessage> PutAsync(HttpRequestMessage message); 
		Task<HttpResponseMessage> PostAsync(HttpRequestMessage message); 
		Task<HttpResponseMessage> PatchAsync(HttpRequestMessage message); 
		Task<HttpResponseMessage> DeleteAsync(HttpRequestMessage message); 
	}
}
