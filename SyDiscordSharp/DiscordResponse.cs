using Http.Enums;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyDiscordSharp
{
    public class DiscordResponse<TResult>
    {
        public bool IsSuccessful { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public JsonErrorCode? JsonError { get; private set; } = null;
        public TResult Result { get; private set; }
        internal static async Task<DiscordResponse<TResult>> ParseAsync(HttpResponseMessage response)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            DiscordResponse<TResult> result = new DiscordResponse<TResult>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Result = response.IsSuccessStatusCode
                       ? JsonConvert.DeserializeObject<TResult>(responseContent)
                       : default
            };

            if (!result.IsSuccessful)
            {
                var jsonErrorDefenition = new
                {
                    message = "",
                    code = ""
                };
                var jsonError = JsonConvert.DeserializeAnonymousType(responseContent, jsonErrorDefenition);
                if (!string.IsNullOrWhiteSpace(jsonError.code))
                {
                    result.JsonError = (JsonErrorCode)Enum.Parse(typeof(JsonErrorCode), jsonError.code);
                }
            }
            return result;
        }
    }
}
