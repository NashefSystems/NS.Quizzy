using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Shared.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace NS.Quizzy.Server.BL.Services
{
    internal class FcmService : IFcmService
    {
        private readonly string _projectId = "quizzy-74a09"; // Replace with your actual project ID
        private readonly string _jsonPath = "firebase-service-account.json"; // Path to your service account JSON
        private readonly INSLogger _logger;

        public FcmService(INSLogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendPushNotificationAsync(PushNotificationRequest request, INSLogBag parentLogBag = null)
        {
            using var logBag = _logger.CreateLogBag(nameof(SendPushNotificationAsync), parentLogBag);
            try
            {

                if (!File.Exists(_jsonPath))
                {
                    throw new FileNotFoundException($"json file '{_jsonPath}' not found");
                }

                var credential = GoogleCredential
                    .FromFile(_jsonPath)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                var accessToken = await credential.UnderlyingCredential
                    .GetAccessTokenForRequestAsync();

                var message = new
                {
                    message = new
                    {
                        token = request.DeviceToken,
                        notification = new
                        {
                            title = request.Title,
                            body = request.Body
                        },
                        data = request.Data
                    }
                };

                var jsonMessage = JsonConvert.SerializeObject(message);
                var url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                var responseContent = await response.Content.ReadAsStringAsync();
                logBag.Trace($"ResponseStatusCode: {response.StatusCode}");
                logBag.Trace($"ResponseContent: {responseContent}");
                if (!response.IsSuccessStatusCode)
                {
                    logBag.LogLevel = NSLogLevel.Error;
                    logBag.AddOrUpdateParameter("ResponseStatusCode", response.StatusCode);
                    logBag.AddOrUpdateParameter("ResponseContent", responseContent);
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                logBag.Exception = ex;
            }
            return false;
        }
    }
}
