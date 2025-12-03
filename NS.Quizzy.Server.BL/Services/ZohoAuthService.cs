using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Shared.CacheProvider.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ZohoAuthService : IZohoAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly INSCacheProvider _cacheProvider;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _refreshToken;
        private readonly string _redirectUri;
        private readonly List<string> _scopes;
        private const string CACHE_KEY = "ZohoAuthService.AccessTokenResponse";

        public ZohoAuthService(IConfiguration configuration, HttpClient httpClient, INSCacheProvider cacheProvider)
        {
            _httpClient = httpClient;
            _cacheProvider = cacheProvider;

            _clientId = configuration.GetValue<string>("Zoho:ClientId", string.Empty);
            _clientSecret = configuration.GetValue<string>("Zoho:ClientSecret", string.Empty);
            _refreshToken = configuration.GetValue<string>("Zoho:RefreshToken", string.Empty);
            _redirectUri = configuration.GetValue<string>("Zoho:RedirectUri", string.Empty);
            _scopes = configuration.GetValue<List<string>>("Zoho:Scopes", []);
        }

        // Generate authorization URL (run once manually)
        public string GetAuthorizationUrl()
        {
            return $"https://accounts.zoho.com/oauth/v2/auth?" +
                   $"scope={string.Join(",", _scopes)}&" +
                   $"client_id={_clientId}&" +
                   $"response_type=code&" +
                   $"access_type=offline&" +
                   $"redirect_uri={Uri.EscapeDataString(_redirectUri)}";
        }

        // Exchange authorization code for refresh token (run once manually)
        public async Task<string> ExchangeCodeForRefreshToken(string authCode)
        {
            var content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id",_clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                new KeyValuePair<string, string>("code", authCode)
            ]);

            var response = await _httpClient.PostAsync(
                "https://accounts.zoho.com/oauth/v2/token", content);
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            return result.GetProperty("refresh_token").GetString();
        }

        // Get access token using refresh token (use in your service)
        public async Task<string> GetAccessToken()
        {
            var accessTokenResponse = await _cacheProvider.GetAsync<AccessTokenResponse>(CACHE_KEY);
            if (accessTokenResponse != null)
            {
                return accessTokenResponse.AccessToken;
            }

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("refresh_token",_refreshToken)
            });

            var response = await _httpClient.PostAsync(
                "https://accounts.zoho.com/oauth/v2/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"({response.StatusCode}) {responseContent}");
            }

            accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(responseContent);
            if (accessTokenResponse == null)
            {
                throw new Exception($"accessTokenResponse is null");
            }
            await _cacheProvider.SetOrUpdateAsync(CACHE_KEY, accessTokenResponse, TimeSpan.FromSeconds(accessTokenResponse.ExpiresIn - 10));
            return accessTokenResponse.AccessToken;
        }

        class AccessTokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }

            [JsonProperty("api_domain")]
            public string ApiDomain { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}
