﻿using API.Enums;
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
        internal delegate void ToLog(string logData);
        internal event ToLog Log = delegate { };
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
        /// Summary: 
        ///     Inicializate connection to discord HTTP server
        ///
        /// Exceptions:
        ///     AuthorizeException:
        ///         Throw when HTTP-client unable to connect to HTTP-server
        public async Task StartAsync()
        {
            bool connected = await TryToConnect();
            if (!connected)
            {
                Log("Cannot connect to HTTP-server. Retrying in 5s");
                await StartAsync();
                return; // TODO : пахнет неладным
            }
                
            bool authorized = await TryToAuthorize();
            if (!authorized)
                throw new Exception("Unable to Authorize"); //TODO : Собственное исключение
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
        private async Task<HttpResponseMessage> Get(string endPoint)
        {
            return await SendAsync(HttpMethods.Get, endPoint, null);
        }
        private async Task<HttpResponseMessage> Put(string endPoint)
        {
            return await SendAsync(HttpMethods.Put, endPoint, null);
        }
        private async Task<HttpResponseMessage> Post(string endPoint, HttpContent content = null)
        {
            return await SendAsync(HttpMethods.Post, endPoint, content);
        }
        private async Task<HttpResponseMessage> Patch(string endPoint, HttpContent content = null)
         {
            return await SendAsync(HttpMethods.Patch, endPoint, content);
        }
        private async Task<HttpResponseMessage> Delete(string endPoint, HttpContent content = null)
        {
            return await SendAsync(HttpMethods.Delete, endPoint, content);
        }
        public async Task<HttpResponseMessage> GetGuildBannedUsers(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/bans";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> GetGuildInvites(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/invites";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> GetMessages(string targetChannelId,
                                                           string messagesRange)
        {
            string endPoint = $"/api/channels/{targetChannelId}/messages{messagesRange}";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> GetMessage(string targetChannelId, string targetMessageId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/messages/{targetMessageId}";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> ModifyCurrentUser(StringContent userNewInfo)
        {
            string endPoint = "/api/users/@me";
            return await Patch(endPoint, userNewInfo);
        }
        public async Task<HttpResponseMessage> ModifyChannel(string targetChannelId, StringContent newChannelInfo)
        {
            string endPoint = $"/api/channels/{targetChannelId}";
            return await Patch(endPoint, newChannelInfo);
        }
        public async Task<HttpResponseMessage> DeleteChannel(string targetChannelId)
        {
            string endPoint = $"/api/channels/{targetChannelId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> BulkDeleteMessages(string targetChannelId, 
                                                                  StringContent messagesIdentifiers)
        {
            string endPoint = $"/api/channels/{targetChannelId}/messages/bulk-delete";
            return await Post(endPoint, messagesIdentifiers);
        }
        public async Task<HttpResponseMessage> CreateReaction(string targetChannelId, 
                                                              string targetMessageId, 
                                                              string emoji)
        {
            string endPoint = $"/api/channels/{targetChannelId}/messages/{targetMessageId}/reactions/{emoji}/@me";
            return await Put(endPoint);
        }
        public async Task<HttpResponseMessage> SendMessage(string targetChannelId, HttpContent message)
        {
            string endPoint = $"/api/channels/{targetChannelId}/messages";
            return await Post(endPoint, message);
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
                    return false;
                else //TODO : выяснить какие ещё причины могут быть
                    return false;
            }
        }
        private (TokenType Type, string Token) GetTokenAndType() //TODO : подтягивать из конфига
        {
            TokenType type = TokenType.Bot;
            string token = "NTU5MDkwMTUzOTM1NjAxNjY1.XJaDSA.IX8ZHPTebYrgzYPJsXyezjA40EQ";
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
            message.Content = content;
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
        private async Task<bool> ValidateToken() //TODO : валидацию
        {
            return false;
        }
    }
}