using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Shared.Logging;
using NS.Shared.Logging.Extensions;
using System.Text;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.Services
{
    internal class OutlookCalendarService : IOutlookCalendarService
    {
        private readonly INSLogger _logger;
        private readonly AppDbContext _appDbContext;
        private readonly string _authRecordKey;
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _redirectUri;
        private readonly string[] _scopes;

        public OutlookCalendarService(INSLogger logger, AppDbContext appDbContext, IConfiguration config)
        {
            _logger = logger;
            _appDbContext = appDbContext;

            _authRecordKey = AppSettingKeys.GraphAuthRecord.GetDBStringValue();
            _tenantId = config.GetValue<string>(AppSettingKeys.GraphTenantId.GetDBStringValue()) ?? string.Empty;
            _clientId = config.GetValue<string>(AppSettingKeys.GraphClientId.GetDBStringValue()) ?? string.Empty;
            _redirectUri = config.GetValue<string>(AppSettingKeys.GraphRedirectUri.GetDBStringValue()) ?? string.Empty;
            _scopes = config.GetValue<string[]>(AppSettingKeys.GraphScopes.GetDBStringValue()) ?? [];
        }

        private async Task<GraphServiceClient> CreateGraphClientAsync()
        {
            var cacheOptions = new TokenCachePersistenceOptions
            {
                Name = "graph_token_cache"
            };

            var authRecord = await LoadAuthRecordAsync();
            var options = new InteractiveBrowserCredentialOptions
            {
                TenantId = _tenantId,
                ClientId = _clientId,
                RedirectUri = new Uri(_redirectUri),
                TokenCachePersistenceOptions = cacheOptions,
                AuthenticationRecord = authRecord
            };

            var credential = new InteractiveBrowserCredential(options);
            // If there is no record yet, force an initial interactive login and save it.
            if (authRecord == null)
            {
                var context = new TokenRequestContext(_scopes);
                var result = credential.Authenticate(context);
                await SaveAuthRecordAsync(result);   // persist the account
            }
            var client = new GraphServiceClient(credential, _scopes);
            return client;
        }

        static async Task<string?> GetCalendarIdAsync(GraphServiceClient graphClient, string calendarName)
        {
            var calendars = await graphClient.Me.Calendars.GetAsync();
            var calendar = calendars?.Value?.FirstOrDefault(c => c.Name == calendarName);
            return calendar?.Id;
        }

        public async Task<EventInCalendar?> CreateOrUpdateEventInCalendar(EventInCalendar eventItem)
        {
            using var logBag = _logger.CreateLogBag(nameof(CreateOrUpdateEventInCalendar));
            try
            {
                ArgumentNullException.ThrowIfNull(eventItem);

                logBag.AddOrUpdateParameter(nameof(eventItem), eventItem.ToMaskedJson());

                var graphClient = await CreateGraphClientAsync();
                var calendarId = await GetCalendarIdAsync(graphClient, eventItem.CalendarName ?? "Quizzy");

                var timeZone = "Israel Standard Time";
                if (!string.IsNullOrWhiteSpace(eventItem.TimeZone))
                {
                    timeZone = eventItem.TimeZone;
                }

                logBag.AddOrUpdateParameter(nameof(timeZone), timeZone);
                var requestBody = new Event
                {
                    Subject = eventItem.Subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = eventItem.Body,
                    },
                    Start = new DateTimeTimeZone
                    {
                        DateTime = eventItem.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        TimeZone = timeZone
                    },
                    Location = new Location
                    {
                        DisplayName = eventItem.Location
                    },
                    IsAllDay = eventItem.IsAllDay,
                    Attendees = []
                };

                var emails = await _appDbContext.EventEmails.Where(x => !x.IsDeleted).ToListAsync();
                emails.ForEach(email =>
                {
                    requestBody.Attendees.Add(new Attendee()
                    {
                        EmailAddress = new EmailAddress()
                        {
                            Address = email.Email,
                            Name = email.Name
                        }
                    });
                });

                if (eventItem.IsAllDay)
                {
                    var startDate = eventItem.StartTime.Date;
                    requestBody.Start = new DateTimeTimeZone
                    {
                        DateTime = startDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                        TimeZone = timeZone
                    };
                    requestBody.End = new DateTimeTimeZone
                    {
                        DateTime = startDate.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"),
                        TimeZone = timeZone
                    };
                }
                else
                {
                    requestBody.Start = new DateTimeTimeZone
                    {
                        DateTime = eventItem.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        TimeZone = timeZone
                    };

                    requestBody.End = new DateTimeTimeZone
                    {
                        DateTime = eventItem.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        TimeZone = timeZone
                    };
                }
                logBag.AddOrUpdateParameter("IsAllDay", requestBody.IsAllDay);

                var response = string.IsNullOrWhiteSpace(eventItem.Id) ?
                    await graphClient.Me.Calendars[calendarId].Events.PostAsync(requestBody) :
                    await graphClient.Me.Calendars[calendarId].Events[eventItem.Id].PatchAsync(requestBody);

                eventItem.Id = response?.Id;
                logBag.AddOrUpdateParameter("EventId", eventItem?.Id);

                return eventItem;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"{nameof(CreateOrUpdateEventInCalendar)} exception", new { eventItem });
                throw;
            }
        }

        private async Task<AuthenticationRecord?> LoadAuthRecordAsync()
        {
            var authRecord = await _appDbContext.AppSettings.FirstOrDefaultAsync(x => x.Key == _authRecordKey);
            if (string.IsNullOrWhiteSpace(authRecord?.Value))
                return null;

            // Convert string -> Stream for AuthenticationRecord.Deserialize
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(authRecord.Value));
            return AuthenticationRecord.Deserialize(ms);
        }

        private async Task SaveAuthRecordAsync(AuthenticationRecord record)
        {
            using var ms = new MemoryStream();
            record.Serialize(ms);
            var json = Encoding.UTF8.GetString(ms.ToArray());
            var authRecord = await _appDbContext.AppSettings.FirstOrDefaultAsync(x => x.Key == _authRecordKey);
            if (authRecord != null)
            {
                authRecord.Value = json;
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}
