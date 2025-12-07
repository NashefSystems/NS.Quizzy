using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace NS.Quizzy.Server.BL.Services
{
    /// <summary>
    /// Zoho help
    /// https://www.zoho.com/calendar/help/api/introduction.html
    /// </summary>
    internal class ZohoCalendarService : IZohoCalendarService
    {
        private readonly IZohoAuthService _authService;
        private readonly HttpClient _httpClient;
        private readonly string _calendarUid;

        private const string BaseUrl = "https://calendar.zoho.com/api/v1";
        private const string DefaultTimeZone = "Asia/Jerusalem"; // IANA timezone for Israel

        public ZohoCalendarService(IZohoAuthService authService, HttpClient httpClient, IConfiguration configuration)
        {
            _authService = authService;
            _httpClient = httpClient;
            _calendarUid = configuration.GetValue<string>("Zoho:CalendarUid", string.Empty);
        }

        // Create or update Event
        public async Task<string> CreateOrUpdateEventAsync(ZohoCalendarEvent calendarEvent, string? _eventId = null)
        {
            var accessToken = await _authService.GetAccessToken();
            if (string.IsNullOrWhiteSpace(calendarEvent.TimeZone))
            {
                calendarEvent.TimeZone = DefaultTimeZone;
            }
            string? uid = null, etag = null;
            if (!string.IsNullOrWhiteSpace(_eventId) && _eventId.Contains("|"))
            {
                var values = _eventId.Split("|");
                if (values.Length == 2)
                {
                    uid = values[0];
                    etag = values[1];
                }
            }


            var eventData = new
            {
                uid,
                etag,
                title = calendarEvent.Title,
                dateandtime = new
                {
                    start = $"{calendarEvent.StartTime:yyyyMMdd}T{calendarEvent.StartTime:HHmmss}",
                    end = $"{calendarEvent.EndTime:yyyyMMdd}T{calendarEvent.EndTime:HHmmss}",
                    timezone = calendarEvent.TimeZone
                },
                isallday = calendarEvent.IsAllDay,
                location = calendarEvent.Location,
                attendees = calendarEvent.Emails?.Select(e => new { email = e,/* status = "NEEDS-ACTION"*/ }).ToList(),
                richtext_description = calendarEvent.Description,
            };
            var eventDataJson = JsonConvert.SerializeObject(eventData, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            });

            var request = string.IsNullOrWhiteSpace(uid) ?
                new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/calendars/{_calendarUid}/events") :
                new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}/calendars/{_calendarUid}/events/{uid}");
            request.Headers.Add("Authorization", $"Zoho-oauthtoken {accessToken}");
            var collection = new List<KeyValuePair<string, string>>
            {
                new("eventdata", eventDataJson)
            };
            request.Content = new FormUrlEncodedContent(collection);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                if (responseBody.Contains("CONFLICT_EVENT"))
                {
                    // if event ID not found
                    return await CreateOrUpdateEventAsync(calendarEvent);
                }
                throw new Exception($"{response.StatusCode} - {responseBody}");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(etag))
            {
                uid = result.GetProperty("events")[0].GetProperty("uid").GetString();
                etag = result.GetProperty("events")[0].GetProperty("etag").GetString();
            }
            return $"{uid}|{etag}";
        }

        // Delete Event
        public async Task DeleteEventAsync(string _eventId)
        {
            var accessToken = await _authService.GetAccessToken();
            string? uid = null, etag = null;
            if (!string.IsNullOrWhiteSpace(_eventId) && _eventId.Contains("|"))
            {
                var values = _eventId.Split("|");
                if (values.Length == 2)
                {
                    uid = values[0];
                    etag = values[1];
                }
            }
            var eventData = new
            {
                uid,
                etag,
            };
            var eventDataJson = JsonConvert.SerializeObject(eventData, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            });

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/calendars/{_calendarUid}/events/{uid}");

            request.Headers.Add("Authorization", $"Zoho-oauthtoken {accessToken}");
            var collection = new List<KeyValuePair<string, string>>
            {
                new("eventdata", eventDataJson)
            };
            request.Content = new FormUrlEncodedContent(collection);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"{response.StatusCode} - {responseBody}");
            }
        }

        // Get Events
        public async Task<List<ZohoCalendarEvent>> GetEventsAsync(DateTime fromDate, DateTime toDate)
        {
            var accessToken = await _authService.GetAccessToken();

            var from = fromDate.ToString("yyyyMMdd");
            var to = toDate.ToString("yyyyMMdd");

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{BaseUrl}/events?fromdate={from}&todate={to}")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", accessToken) }
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            // Parse and return events
            var events = new List<ZohoCalendarEvent>();
            // ... parse the response
            return events;
        }
    }
}
