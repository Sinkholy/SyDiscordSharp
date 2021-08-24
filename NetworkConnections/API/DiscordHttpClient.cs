using API;
using API.Enums;

using Http;
using Http.Connection;

using HttpCommunication.Connection;

using JsonHandler;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HttpCommunication
{
    public partial class DiscordHttpClient
	{
        readonly EndpointsGenerator endpointsGenerator;
        readonly IHttpConnection httpConnection;

        public DiscordHttpClient(IHttpConnection httpClient, ConnectionParameters parameters, BotHttpToken token)
        {
            Token = token;
            bool validToken = ValidateToken(Token);
            if (!validToken)
            {
                var message = "Provided token isnt valid."; // TODO: добавить правильную структуру токена
                var exepction = new ArgumentException(message, "token");
                throw exepction;
            }
            ApiAddress = parameters.BaseAddress;
            ApiVersion = parameters.ApiVersion;
            endpointsGenerator = new EndpointsGenerator();
            httpConnection = httpClient;
            PrepareHttpConnection();

            void PrepareHttpConnection()
			{
                httpConnection.Headers.AuthorizationHeader = CreateAuthorizationHeader();
                httpConnection.Headers.UserAgentHeader = CreateUserAgentHeader();
                httpConnection.BaseAdress = CreateBaseAddressWithApiVersion();

                Header CreateAuthorizationHeader()
                {
                    string authorizationHeaderParameter = $"{token.Type} {token.Value}";
                    return new Header("Authorization", authorizationHeaderParameter);
                }
                Header CreateUserAgentHeader()
                {
                    //User - Agent: DiscordBot($url, $versionNumber)
                    // TODO: протестировать как это должно выглядеть
                    // У меня два варианта:
                    // 1 - на место "DiscordBot" нужно устанавливать имя библиотеки
                    // 2 - на место $url нужно устанавливать имя библиотеки
                    var headerParameter = $"{parameters.LibraryName} ({parameters.BaseAddress}, {parameters.ApiVersion})";
                    return new Header("User - Agent", headerParameter);
                }
                Uri CreateBaseAddressWithApiVersion()
				{
                    return new Uri(parameters.BaseAddress, $"/{parameters.ApiVersion}");
				}
            }
        }

        public Uri ApiAddress { get; private set; }
        public int ApiVersion { get; private set; }
        public BotHttpToken Token { get; private set; }

        public async Task<AuthenticationResult> AuthenticateAsync()
		{
            string endpoint = endpointsGenerator.GenerateAuthorizationEndpoint();
            var message = new Connection.HttpRequestMessage(endpoint, null);
			Connection.HttpResponseMessage responseMessage = await httpConnection.GetAsync(message);
            var result = new AuthenticationResult(responseMessage.IsSuccessStatusCode, responseMessage.ReasonPhrase);
            return result;
        }
        public async Task<GatewayInfo> GetGatewayBotInfoAsync()
		{
            string endpoint = endpointsGenerator.GenerateGatewayBotEndpoint();
            var message = new Connection.HttpRequestMessage(endpoint, null);
            Connection.HttpResponseMessage responseMessage = await httpConnection.GetAsync(message);
            GatewayInfo result = await Deserialize();
            return result;

            async Task<GatewayInfo> Deserialize()
			{
                string receivedResult = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GatewayInfo>(receivedResult);
			}
        }

        bool ValidateToken(BotHttpToken target)
		{
            // Провалидировать токен
            // Вернуть результат
            return true; // UNDONE: заглушка
		}

        class EndpointsGenerator
		{
            const string Users = "users";
            const string Me = "@me";
            const string Gateway = "gateway";
            const string Bot = "bot";
            const string Channels = "channels";
            const string Messages = "messages";
            // Другие промежуточные\конечные пункты
            const string Splitter = "/";

            readonly StringBuilder combiner;

            internal EndpointsGenerator()
			{
                combiner = new StringBuilder();
			}

			internal string GenerateAuthorizationEndpoint()
			{
                return Combine(Users, Me);
			}
            internal string GenerateGatewayBotEndpoint()
			{
                return Combine(Gateway, Bot);
			}

            string Combine(params string[] components)
			{
                combiner.Clear();
				foreach (string component in components)
				{
                    combiner.Append(Splitter);
                    combiner.Append(component);
                }
                return combiner.ToString();
            }
        }
        public class ConnectionParameters
        {
            public Uri BaseAddress { get; private set; }
            public int ApiVersion { get; private set; }
            public string LibraryName { get; private set; }
        }
    }

    public partial class DiscordHttpClient
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
        private async Task<HttpResponseMessage> Put(string endPoint, HttpContent content = null)
        {
            return await SendAsync(HttpMethods.Put, endPoint, content);
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
        public async Task<HttpResponseMessage> ModifyCurrentUser(string jsonUserInfo)
        {
            string endPoint = "/api/users/@me";
            StringContent content = new StringContent(jsonUserInfo, Encoding.UTF8, "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> GetUserGuilds(string targetUserId, 
                                                             string beforeGuildId, 
                                                             string afterGuildId, 
                                                             int limit = 100)
        {
            string endPoint = $"/api/users/{targetUserId}/guilds"
                .AddQueryParameters(new (string, string)[] { ("before", beforeGuildId), 
                                                             ("after", afterGuildId), 
                                                             ("limit", limit.ToString()) });
            return await Get(endPoint);
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
        public async Task<HttpResponseMessage> DeleteReaction(string targetChannelId,
                                                              string targetMessageId,
                                                              string emoji,
                                                              string targetUserId)
        {
            StringBuilder endPoint = new StringBuilder($"/api/channels/{targetChannelId}/messages/{targetMessageId}/reactions");
            if (!string.IsNullOrWhiteSpace(emoji))
            {
                endPoint.Append($"/{emoji}");
            }
            if (!string.IsNullOrWhiteSpace(targetUserId))
            {
                endPoint.Append($"/{targetUserId}");
            }
            return await Delete(endPoint.ToString());
        }
        public async Task<HttpResponseMessage> GetReactions(string targetChannelId,
                                                            string targetMessageId,
                                                            string emoji)
        {
            string endPoint = $"/api/channels/{targetChannelId}/messages/{targetMessageId}/reactions/{emoji}";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> EditMessage(string targetChannelId,
                                                           string targetMessageId, 
                                                           string jsonMessageData)
        {
            string endPoint = $"/api/channels/{targetChannelId}/messages/{targetMessageId}";
            StringContent content = new StringContent(jsonMessageData,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> DeleteMessage(string targetChannelId,
                                                             string targetMessageId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/messages/{targetMessageId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> GetChannelInvites(string targetChannelId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/invites";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> CreateChannelInvite(string targetChannelId,
                                                                   string inviteJson)
        {
            string endPoint = $"/api/channels/{targetChannelId}/invites";
            StringContent content = new StringContent(inviteJson,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> DeleteChannelPermission(string targetChannelId,
                                                                       string targetOverwriteId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/permissions/{targetOverwriteId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> FollowNewsChannel(string targetChannelId, string jsonTargetWebhookChannelId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/followers";
            StringContent content = new StringContent(jsonTargetWebhookChannelId,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> EditChannelPermissions(string targetChannelId, 
                                                                      string targetOverwriteId, 
                                                                      string jsonOverwriteData)
        {
            string endPoint = $"/api/channels/{targetChannelId}/permissions/{targetOverwriteId}";
            StringContent content = new StringContent(jsonOverwriteData,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Put(endPoint, content);
        }
        public async Task<HttpResponseMessage> TriggerTypingIndicator(string targetChannelId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/typing";
            return await Post(endPoint);
        }
        public async Task<HttpResponseMessage> GetPinnedMessages(string targetChannelId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/pins";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> DeletePinnedMessage(string targetChannelId,
                                                                   string targetMessageId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/pins/{targetMessageId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> PinMessage(string targetChannelId,
                                                          string targetMessageId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/pins/{targetMessageId}";
            return await Put(endPoint);
        }
        public async Task<HttpResponseMessage> RemoveUserFromGroupDm(string targetChannelId,
                                                                     string targetUserId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/recipients/{targetUserId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> GetGuildEmoji(string targetGuildId,
                                                             string targetEmojiId)
        {
            StringBuilder endPoint = new StringBuilder($"/api/guilds/{targetGuildId}/emojis");
            if (!string.IsNullOrWhiteSpace(targetEmojiId))
            {
                endPoint.Append($"/{targetEmojiId}");
            }
            return await Get(endPoint.ToString());
        }
        public async Task<HttpResponseMessage> CreateGuildEmoji(string targetGuildId,
                                                                string emojiJson)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/emojis";
            StringContent content = new StringContent(emojiJson,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> ModifyGuildEmoji(string targetGuildId,
                                                                string targetEmojiId,
                                                                string emojiJson)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/emojis/{targetEmojiId}";
            StringContent content = new StringContent(emojiJson,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> GetInvite(string targetInviteCode)
        {
            string endPoint = $"/api/invites/{targetInviteCode}";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> DeleteInvite(string targetInviteCode)
        {
            string endPoint = $"/api/invites/{targetInviteCode}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> GetUser(string targetUserId)
        {
            string endPoint = $"/api/users/{targetUserId}";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> DeleteGuildEmoji(string targetGuildId, string targetEmojiId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/emojis/{targetEmojiId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> LeaveGuild(string targetGuildId)
        {
            string endPoint = $"/api/users/@me/guilds/{targetGuildId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> GetUserDMs()
        {
            string endPoint = $"/api/users/@me/channels";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> CreateDM(string jsonUsers)
        {
            string endPoint = $"/api/users/@me/channels";
            StringContent content = new StringContent(jsonUsers,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> GetUserConnections(string targetUser)
        {
            string endPoint = $"/api/users/{targetUser}/connections";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> GetVoiceRegions()
        {
            string endPoint = $"/api/voice/regions";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> CreateGuild(string jsonGuild)
        {
            string endPoint = $"/api/guilds";
            StringContent content = new StringContent(jsonGuild,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> ModifyGuild(string targetGuildId, string jsonGuildInfo)
        {
            string endPoint = $"/api/guilds/{targetGuildId}";
            StringContent content = new StringContent(jsonGuildInfo,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> DeleteGuild(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> GetGuild(string targetGuildId, bool preview, bool withCounts = false)
        {
            StringBuilder endPoint = new StringBuilder($"/api/guilds/{targetGuildId}");
            if (preview)
            {
                endPoint.Append("/preview");
            }
            else
            {
                if (withCounts)
                {
                    endPoint.AddQueryParameters(new (string name, string value)[] { ("with_counts", "true") });
                }
            }
            return await Get(endPoint.ToString());
        }
        public async Task<HttpResponseMessage> GetGuildChannels(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/channels";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> CreateGuildChannel(string targetGuildId, string channelJson)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/channels";
            StringContent content = new StringContent(channelJson,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> ModifyGuildChannelPositions(string targetGuildId, string json)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/channels";
            StringContent content = new StringContent(json,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> GetGuildUsers(string targetGuildId, string firstUserId, int limit = 1)
        {
            string endPoint;
            if(limit == 1)
            {
                endPoint = $"/api/guilds/{targetGuildId}/members/{firstUserId}";
            }
            else
            {
                endPoint = $"/api/guilds/{targetGuildId}/members"
                    .AddQueryParameters(new (string name, string value)[] { ("after", firstUserId), ("limit", limit.ToString()) });
            }
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> AddGuildUser(string targetGuildId, 
                                                            string targetUserId, 
                                                            string jsonUserData)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/members/{targetUserId}";
            StringContent content = new StringContent(jsonUserData,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Put(endPoint, content);
        }
        public async Task<HttpResponseMessage> ModifyGuildMember(string targetGuildId,
                                                                 string targetUserId,
                                                                 string jsonParams)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/members/{targetUserId}";
            StringContent content = new StringContent(jsonParams,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> ModifyCurrentUserNickname(string targetGuildId, string jsonNickname)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/members/@me/nick";
            StringContent content = new StringContent(jsonNickname,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> AddGuildMemberRole(string targetGuildId,
                                                                  string targetUserId,
                                                                  string targetRoleId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/members/{targetUserId}/roles/{targetRoleId}";
            return await Put(endPoint);
        }
        public async Task<HttpResponseMessage> RemoveGuildMemberRole(string targetGuildId,
                                                                     string targetUserId,
                                                                     string targetRoleId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/members/{targetUserId}/roles/{targetRoleId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> RemoveGuildMember(string targetGuildId,
                                                                 string targetUserId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/members/{targetUserId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> GetGuildBan(string targetGuildId, string targetBanId = null)
        {
            StringBuilder endPoint = new StringBuilder($"/api/guilds/{targetGuildId}/bans");
            if (targetBanId != null)
            {
                endPoint.Append($"/{targetBanId}");
            }
            return await Get(endPoint.ToString());
        }
        public async Task<HttpResponseMessage> CreateGuildBan(string targetGuildId,
                                                              string targetUserId,
                                                              string jsonParameters)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/bans/{targetUserId}";
            StringContent content = new StringContent(jsonParameters,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Put(endPoint, content);
        }
        public async Task<HttpResponseMessage> RemoveGuildBan(string targetGuildId,
                                                              string targetUserId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/bans/{targetUserId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> GetGuildRoles(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/roles";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> CreateGuildRole(string targetGuildId, string jsonRoleData)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/roles";
            StringContent content = new StringContent(jsonRoleData,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> ModifyGuildRolePosition(string targetGuildId, string jsonParams)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/roles";
            StringContent content = new StringContent(jsonParams,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> ModifyGuildRole(string targetGuildId,
                                                               string targetRoleId,
                                                               string jsonRoleData)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/roles/{targetRoleId}";
            StringContent content = new StringContent(jsonRoleData,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> DeleteGuildRole(string targetGuildId, string targetRoleId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/roles/{targetRoleId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> GetGuildPruneCount(string targetGuildId, 
                                                                  int days = 7, 
                                                                  string[] includedRoles = null)
        {
            string includedRolesQueryParam = null;
            if (includedRoles?.Length != 0)
            {
                includedRolesQueryParam = includedRoles.Aggregate((x, y) => $"{x},{y}");
            }
            string endPoint = $"/api/guilds/{targetGuildId}/prune"
                .AddQueryParameters(new (string name, string value)[] { ("days", days.ToString()), 
                                                                        ("include_roles", includedRolesQueryParam ) });
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> BeginGuildPrune(string targetGuildId, string jsonPruneData)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/prune";
            StringContent content = new StringContent(jsonPruneData,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> GetGuildVoiceRegions(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/regions";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> GetGuildIntegrations(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/integrations";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> CreateGuildIntegration(string targetGuildId, string jsonIntegrationData)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/integrations";
            StringContent content = new StringContent(jsonIntegrationData,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> ModifyGuildIntegration(string targetGuildId, 
                                                                      string targetIntegrationId, 
                                                                      string jsonIntegrationData)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/integrations/{targetIntegrationId}";
            StringContent content = new StringContent(jsonIntegrationData,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint, content);
        }
        public async Task<HttpResponseMessage> DeleteGuildIntegration(string targetGuildId, string targetIntegrationId) 
        {
            string endPoint = $"/api/guilds/{targetGuildId}/integrations/{targetIntegrationId}";
            return await Delete(endPoint);
        }
        public async Task<HttpResponseMessage> SyncGuildIntegration(string targetGuildId, string targetIntegrationId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/integrations/{targetIntegrationId}/sync";
            return await Post(endPoint);
        }
        public async Task<HttpResponseMessage> GetGuildVanityURL(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/vanity-url";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> CreateWebhook(string targetChannelId, string jsonWebhookData)
        {
            string endPoint = $"/api/channels/{targetChannelId}/webhooks";
            StringContent content = new StringContent(jsonWebhookData,
                                                      Encoding.UTF8,
                                                      "application/json");
            var bytesCont = new ByteArrayContent();
            return await Post(endPoint, content);
        }
        public async Task<HttpResponseMessage> GetChannelWebhooks(string targetChannelId)
        {
            string endPoint = $"/api/channels/{targetChannelId}/webhooks";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> GetGuildWebhooks(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/webhooks";
            return await Get(endPoint);
        }
        public async Task<HttpResponseMessage> GetWebhook(string targetWebhookId, string token = null)
        {
            StringBuilder endPoint = new StringBuilder($"/api/webhooks/{targetWebhookId}");
            if (token != null)
            {
                endPoint.Append($"/{token}");
            }
            return await Get(endPoint.ToString());
        }
        public async Task<HttpResponseMessage> ModifyWebhook(string targetWebhookId, 
                                                             string jsonWebhookData, 
                                                             string token = null)
        {
            StringBuilder endPoint = new StringBuilder($"/api/webhooks/{targetWebhookId}");
            if(token != null)
            {
                endPoint.Append($"/{token}");
            }
            StringContent content = new StringContent(jsonWebhookData,
                                                      Encoding.UTF8,
                                                      "application/json");
            return await Patch(endPoint.ToString(), content);
        }
        public async Task<HttpResponseMessage> DeleteWebhook(string targetWebhookId, string token = null)
        {
            StringBuilder endPoint = new StringBuilder($"/api/webhooks/{targetWebhookId}");
            if (token != null)
            {
                endPoint.Append($"/{token}");
            }
            return await Delete(endPoint.ToString());
        }
        public async Task<HttpResponseMessage> GetAuditLog(string targetGuildId)
        {
            string endPoint = $"/api/guilds/{targetGuildId}/audit-logs";
            return await Get(endPoint);
        }


        public async Task<HttpResponseMessage> SendMessage(string targetChannelId, HttpContent message)
        {
            string endPoint = $"/api/channels/{targetChannelId}/webhooks";
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
            string token = "NTU5MDkwMTUzOTM1NjAxNjY1.XJaDSA.XqQEemlTAVeK_ir_hO7gOo73jmk";
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