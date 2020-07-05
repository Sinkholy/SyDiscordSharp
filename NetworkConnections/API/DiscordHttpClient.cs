using API.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class DiscordHttpClient
    {
        private readonly HttpClient httpClient;
        public DiscordHttpClient()
        {
            Uri baseApiUri = GetBaseApiUri();
            (TokenType Type, string Token) botTokenAndType = GetTokenAndType();
            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue(botTokenAndType.Type.ToString(), botTokenAndType.Token);
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = authHeader;
            httpClient.BaseAddress = baseApiUri;
            //TODO : таймаут для запроса
        }
        public async Task StartAsync()//TODO : нормальные исключения
        {
            bool connected = await TryToConnect();
            if (!connected)
                throw new Exception("Connect");

            bool authorized = await TryToAuthorize();
            if (!authorized)
                throw new Exception("Auth");
        }
        public async Task<GatewayInfo> GetGatewayInfoAsync()
        {
            HttpResponseMessage response = await SendAsync(HttpMethods.Get, "/api/gateway/bot");
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            {
                string content = new StreamReader(stream).ReadToEnd();
                return JsonConvert.DeserializeObject<GatewayInfo>(content);
            }
        }
        private async Task<bool> TryToConnect() //TODO : реализовать проверку подключения к дискорду
        {
            await Task.Delay(1);
            return true;
        }
        private async Task<bool> TryToAuthorize()
        {
            HttpResponseMessage response = await SendAsync(HttpMethods.Get, "/api/users/@me");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                if (response.ReasonPhrase == "Unauthorized")
                {
                    return false;
                }
                else // TODO : исключение?
                {
                    return false; //Заглушка
                }
            }
        }
        private (TokenType Type, string Token) GetTokenAndType() //TODO : подтягивать из конфига
        {
            TokenType type = TokenType.Bot;
            string token = "NTU5MDkwMTUzOTM1NjAxNjY1.XtKPog.7epgH4xS8QxLqGgiGyBLCladnyI";
            return (Type: type, Token: token);
        }
        private Uri GetBaseApiUri()
        {
            return new Uri("https://discordapp.com/"); //TODO : подтягивать из конфига
        }
        private async Task<HttpResponseMessage> SendAsync(HttpMethods method, string endPoint, HttpContent content = null) //TODO : Проверить что-либо кроме GET запроса
        {
            Uri requestUri = new Uri(httpClient.BaseAddress, endPoint);
            HttpRequestMessage message = new HttpRequestMessage 
            {
                RequestUri = requestUri,
                Method = GetMethod(method)
            };
            if (content is ByteArrayContent)
                message.Content = new ByteArrayContent(content.ReadAsByteArrayAsync().Result);
            else if (content is StreamContent)
                message.Content = new StreamContent(content.ReadAsStreamAsync().Result);
            else if (content is StringContent)
                message.Content = new StringContent(content.ReadAsStringAsync().Result);
            else if (content != null)
                throw new ArgumentOutOfRangeException($"Unknown content type {content}");
            return await this.httpClient.SendAsync(message);
        }
        private static HttpMethod GetMethod(HttpMethods method)
        {
            switch (method)
            {
                case HttpMethods.Get: return HttpMethod.Get;
                case HttpMethods.Post: return HttpMethod.Post;
                case HttpMethods.Delete: return HttpMethod.Delete;
                case HttpMethods.Patch: return new HttpMethod("PATCH"); 
                case HttpMethods.Put: return HttpMethod.Put;
                default: throw new ArgumentOutOfRangeException($"Unknown HttpMethod {method}");
            }
        }
        public async Task<bool> ValidateToken() //TODO : валидацию
        {
            return false;
        }
    }
}