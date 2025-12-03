using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Applications.GetAvailableExtensionProperties;
using Microsoft.Graph.Drives.Item.Items.Item.Workbook.Functions.Days360;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NS.Quizzy.Server.BL.Extensions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.Logging;
using NS.Shared.QueueManager.Models;
using NS.Shared.QueueManager.Services;
using System.Text;

namespace NS.Quizzy.Server.BL.QueueSubscriptions
{
    internal class ExamEventsQueueSubscription : QueueSubscriptionBase
    {
        public override int GetMaximumAttempts() => 2;
        public override string GetVirtualHost() => BLConsts.QUEUE_VIRTUAL_HOST;
        public override string GetQueueName() => BLConsts.QUEUE_EXAM_EVENTS;

        public override async Task<QueueSubscriptionAcceptMethodResult> ProcessMessageAsync(Guid messageId, QueueMessage message, INSLogBag logBag, IServiceScope scope, CancellationToken cancellationToken)
        {
            var res = new QueueSubscriptionAcceptMethodResult();
            string messageStatusCacheKey = messageId.GetQueueMessageStatusCacheKey();
            MessageStatusInfo? messageStatusInfo = null;
            INSCacheProvider? _cacheProvider = null;
            try
            {
                logBag.Trace("Starting ProcessMessageAsync");
                logBag.AddOrUpdateParameter(nameof(messageStatusCacheKey), messageStatusCacheKey);

                _cacheProvider = scope.ServiceProvider.GetRequiredService<INSCacheProvider>();
                messageStatusInfo = await _cacheProvider.GetAsync<MessageStatusInfo>(messageStatusCacheKey);
                messageStatusInfo ??= new MessageStatusInfo();
                messageStatusInfo.DownloadCounter += 1;
                await SetMessageProgressPercentageAsync(_cacheProvider, messageStatusCacheKey, messageStatusInfo, 0);

                #region logic
                var examId = JsonConvert.DeserializeObject<Guid?>(message.Payload);
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var exam = await dbContext.Exams.FirstOrDefaultAsync(x => x.Id == examId);
                if (exam == null)
                {
                    logBag.LogLevel = NSLogLevel.Error;
                    return res.SetError($"Exam ID '{examId}' not found");
                }

                var outlookCalendarService = scope.ServiceProvider.GetRequiredService<IOutlookCalendarService>();
                logBag.AddOrUpdateParameter("ExamIsDeleted", exam.IsDeleted);
                logBag.AddOrUpdateParameter("ExamIsVisible", exam.IsVisible);
                logBag.AddOrUpdateParameter("ExamOutlookCalendarId", exam.OutlookCalendarId);
                await SetMessageProgressPercentageAsync(_cacheProvider, messageStatusCacheKey, messageStatusInfo, 10);

                if (exam.IsVisible && !exam.IsDeleted)
                {
                    logBag.Trace("Create or update event");
                    var classes = (await dbContext.ClassExams
                        .Include(x => x.Class)
                        .Where(x => x.IsDeleted == false && x.ExamId == exam.Id && x.Class.IsDeleted == false)
                        .ToListAsync())
                        .Select(x => new GradeClassInfo(x.Class.Name, x.IsImprovement))
                        .OrderBy(x => x.Name)
                        .ToList();
                    var grades = (await dbContext.GradeExams
                        .Include(x => x.Grade)
                        .Where(x => x.IsDeleted == false && x.ExamId == exam.Id && x.Grade.IsDeleted == false)
                        .ToListAsync())
                        .Select(x => new GradeClassInfo(x.Grade.Name, x.IsImprovement))
                        .OrderBy(x => x.Name)
                        .ToList();
                    var location = string.Join(", ", grades.Union(classes).Select(x => x.Name).ToList());
                    var examType = await dbContext.ExamTypes.FirstOrDefaultAsync(x => x.Id == exam.ExamTypeId);
                    var moed = await dbContext.Moeds.FirstOrDefaultAsync(x => x.Id == exam.MoedId);
                    var questionnaire = await dbContext.Questionnaires.FirstOrDefaultAsync(x => x.Id == exam.QuestionnaireId);
                    var subject = questionnaire?.SubjectId == null ? null : await dbContext.Subjects.FirstOrDefaultAsync(x => x.Id == questionnaire.SubjectId);
                    var body = GetBody(exam, moed, examType, questionnaire, subject, grades, classes);
                    var eventItem = new EventInCalendar()
                    {
                        Id = exam.OutlookCalendarId,
                        Subject = $"{subject?.Name} ({questionnaire?.Code}) - {examType?.Name} - {exam.Duration:hh\\:mm} / {exam.DurationWithExtra:hh\\:mm}",
                        Location = location,
                        Body = body,
                        IsAllDay = exam.StartTime.Hour == 0 && exam.StartTime.Minute == 0,
                        StartTime = exam.StartTime.DateTime,
                        EndTime = exam.StartTime.DateTime.Add(exam.DurationWithExtra),
                    };
                    eventItem = await outlookCalendarService.CreateOrUpdateEventInCalendar(eventItem);
                    exam.OutlookCalendarId = eventItem?.Id;
                }
                else if (!string.IsNullOrWhiteSpace(exam.OutlookCalendarId))
                {
                    logBag.Trace("Delete event");
                    await outlookCalendarService.DeleteEventInCalendar(exam.OutlookCalendarId);
                    exam.OutlookCalendarId = null;
                }
                await SetMessageProgressPercentageAsync(_cacheProvider, messageStatusCacheKey, messageStatusInfo, 90);
                await dbContext.SaveChangesAsync();
                #endregion

                await SuccessHandler(_cacheProvider, messageStatusCacheKey, messageStatusInfo);
                return res.SetOk();
            }
            catch (Exception ex)
            {
                logBag.Exception = ex;
                await ErrorHandlerAsync(_cacheProvider, logBag.Logger, messageStatusCacheKey, messageStatusInfo, ex.Message);
                return res.SetError(ex.Message);
            }
        }

        private string GetBody(Exam exam, Moed moed, ExamType? examType, Questionnaire? questionnaire, Subject? subject, List<GradeClassInfo> grades, List<GradeClassInfo> classes)
        {
            string[] days = ["ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת"];
            var startTimeStr = exam.StartTime.DateTime.ToString(
                (exam.StartTime.Hour == 0 && exam.StartTime.Minute == 0) ?
                    "dd/MM/yyyy" : "dd/MM/yyyy HH:mm"
            );
            var examDay = days[(int)exam.StartTime.DateTime.DayOfWeek];

            var sb = new StringBuilder();
            #region header
            sb.AppendLine("<div dir='rtl'>");
            sb.AppendLine("    <div>");
            sb.AppendLine("        <div style='font-weight: 700; color: #393945;'>");
            sb.AppendLine($"            ({questionnaire?.Code}) {questionnaire?.Name}");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div style='font-size: .9rem; color: #75757d;'>");
            sb.AppendLine($"            {startTimeStr} {examDay} | {examType?.Name} - {moed?.Name}");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <br>");
            sb.AppendLine("    <div");
            sb.AppendLine("        style='border-top: 1px dotted #75757D; padding-top: 0.5rem; display: flex; flex-direction: column; gap: 0.5rem;'>");
            #endregion

            #region subject
            sb.AppendLine("        <div");
            sb.AppendLine("            style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; max-width: 100%; width: 100%;'>");
            sb.AppendLine("            <div");
            sb.AppendLine("                style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; width: 100%; flex-wrap: wrap;'>");
            sb.AppendLine("                <span style='width: 1.5rem; max-width: 1.5rem;'>📚</span>");
            sb.AppendLine("                <span style='color: #393945;'>");
            sb.AppendLine("                    נושא:");
            sb.AppendLine("                </span>");
            sb.AppendLine("                <span style='color: #5538D8; font-weight: bold; text-wrap-mode: wrap;'>");
            sb.AppendLine($"                    {subject?.Name}");
            sb.AppendLine("                </span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            #endregion

            #region duration
            sb.AppendLine("        <div");
            sb.AppendLine("            style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; max-width: 100%; width: 100%;'>");
            sb.AppendLine("            <div");
            sb.AppendLine("                style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; width: 100%; flex-wrap: wrap;'>");
            sb.AppendLine("                <span style='width: 1.5rem; max-width: 1.5rem;'>⏰</span>");
            sb.AppendLine("                <span style='color: #393945;'>");
            sb.AppendLine("                    משך הבחינה:");
            sb.AppendLine("                </span>");
            sb.AppendLine("                <span style='color: #5538D8; font-weight: bold; text-wrap-mode: wrap;'>");
            sb.AppendLine(exam.Duration.ToString(@"hh\:mm"));
            sb.AppendLine("                </span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            #endregion

            #region duration with extra
            sb.AppendLine("        <div");
            sb.AppendLine("            style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; max-width: 100%; width: 100%;'>");
            sb.AppendLine("            <div");
            sb.AppendLine("                style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; width: 100%; flex-wrap: wrap;'>");
            sb.AppendLine("                <span style='width: 1.5rem; max-width: 1.5rem;'>⏰</span>");
            sb.AppendLine("                <span style='color: #393945;'>");
            sb.AppendLine("                    משך הבחינה כולל תוספת זמן:");
            sb.AppendLine("                </span>");
            sb.AppendLine("                <span style='color: #5538D8; font-weight: bold; text-wrap-mode: wrap;'>");
            sb.AppendLine(exam.DurationWithExtra.ToString(@"hh\:mm"));
            sb.AppendLine("                </span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            #endregion

            #region grades
            {
                var items = grades.Where(x => x.IsImprovement == false).Select(x => x.Name).ToList();
                if (items.Count != 0)
                {
                    var value = string.Join(" | ", items);
                    sb.AppendLine("        <div");
                    sb.AppendLine("            style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; max-width: 100%; width: 100%;'>");
                    sb.AppendLine("            <div");
                    sb.AppendLine("                style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; width: 100%; flex-wrap: wrap;'>");
                    sb.AppendLine("                <span style='width: 1.5rem; max-width: 1.5rem;'>👥</span>");
                    sb.AppendLine("                <span style='color: #393945;'>");
                    sb.AppendLine("                    שכבות:");
                    sb.AppendLine("                </span>");
                    sb.AppendLine("                <span style='color: #5538D8; font-weight: bold; text-wrap-mode: wrap;'>");
                    sb.AppendLine($"                    {value}");
                    sb.AppendLine("                </span>");
                    sb.AppendLine("            </div>");
                    sb.AppendLine("        </div>");
                }
            }
            #endregion

            #region improvement grades
            {
                var items = grades.Where(x => x.IsImprovement == true).Select(x => x.Name).ToList();
                if (items.Count != 0)
                {
                    var value = string.Join(" | ", items);
                    sb.AppendLine("        <div");
                    sb.AppendLine("            style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; max-width: 100%; width: 100%;'>");
                    sb.AppendLine("            <div");
                    sb.AppendLine("                style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; width: 100%; flex-wrap: wrap;'>");
                    sb.AppendLine("                <span style='width: 1.5rem; max-width: 1.5rem;'>👥</span>");
                    sb.AppendLine("                <span style='color: #393945;'>");
                    sb.AppendLine("                    שכבות (שיפור ציון):");
                    sb.AppendLine("                </span>");
                    sb.AppendLine("                <span style='color: #5538D8; font-weight: bold; text-wrap-mode: wrap;'>");
                    sb.AppendLine($"                    {value}");
                    sb.AppendLine("                </span>");
                    sb.AppendLine("            </div>");
                    sb.AppendLine("        </div>");
                }
            }
            #endregion

            #region classes
            {
                var items = classes.Where(x => x.IsImprovement == false).Select(x => x.Name).ToList();
                if (items.Count != 0)
                {
                    var value = string.Join(" | ", items);
                    sb.AppendLine("        <div");
                    sb.AppendLine("            style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; max-width: 100%; width: 100%;'>");
                    sb.AppendLine("            <div");
                    sb.AppendLine("                style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; width: 100%; flex-wrap: wrap;'>");
                    sb.AppendLine("                <span style='width: 1.5rem; max-width: 1.5rem;'>🎓</span>");
                    sb.AppendLine("                <span style='color: #393945;'>");
                    sb.AppendLine("                    כיתות:");
                    sb.AppendLine("                </span>");
                    sb.AppendLine("                <span style='color: #5538D8; font-weight: bold; text-wrap-mode: wrap;'>");
                    sb.AppendLine($"                    {value}");
                    sb.AppendLine("                </span>");
                    sb.AppendLine("            </div>");
                    sb.AppendLine("        </div>");
                }
            }
            #endregion

            #region improvement classes
            {
                var items = classes.Where(x => x.IsImprovement == true).Select(x => x.Name).ToList();
                if (items.Count != 0)
                {
                    var value = string.Join(" | ", items);
                    sb.AppendLine("        <div");
                    sb.AppendLine("            style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; max-width: 100%; width: 100%;'>");
                    sb.AppendLine("            <div");
                    sb.AppendLine("                style='display: flex; flex-direction: row; justify-content: flex-start; gap: 0.5rem; width: 100%; flex-wrap: wrap;'>");
                    sb.AppendLine("                <span style='width: 1.5rem; max-width: 1.5rem;'>🎓</span>");
                    sb.AppendLine("                <span style='color: #393945;'>");
                    sb.AppendLine("                    כיתות (שיפור ציון):");
                    sb.AppendLine("                </span>");
                    sb.AppendLine("                <span style='color: #5538D8; font-weight: bold; text-wrap-mode: wrap;'>");
                    sb.AppendLine($"                    {value}");
                    sb.AppendLine("                </span>");
                    sb.AppendLine("            </div>");
                    sb.AppendLine("        </div>");
                }
            }
            #endregion

            #region footer
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
            #endregion

            return sb.ToString();
        }

        private async Task SuccessHandler(INSCacheProvider cacheProvider, string cacheKey, MessageStatusInfo? statusInfo)
        {
            statusInfo ??= new MessageStatusInfo();
            statusInfo.Error = null;
            statusInfo.IsCompleted = true;
            statusInfo.IsSuccess = true;
            statusInfo.ProgressPercentage = 100;
            await UpdateMessageStatusInfoAsync(cacheProvider, cacheKey, statusInfo);
        }

        private async Task ErrorHandlerAsync(INSCacheProvider? cacheProvider, INSLogger? logger, string cacheKey, MessageStatusInfo? statusInfo, string error)
        {
            try
            {
                if (cacheProvider == null)
                {
                    return;
                }
                statusInfo ??= new MessageStatusInfo();
                statusInfo.Error = error;
                statusInfo.IsCompleted = true;
                statusInfo.IsSuccess = false;
                await UpdateMessageStatusInfoAsync(cacheProvider, cacheKey, statusInfo);
            }
            catch (Exception ex)
            {
                logger?.Fatal(ex, nameof(ErrorHandlerAsync), new { cacheKey });
            }
        }

        private async Task SetMessageProgressPercentageAsync(INSCacheProvider cacheProvider, string cacheKey, MessageStatusInfo statusInfo, double progressPercentage)
        {
            if (progressPercentage < 0) progressPercentage = 0;
            if (progressPercentage > 100) progressPercentage = 100;
            statusInfo.ProgressPercentage = Math.Truncate(progressPercentage);
            await UpdateMessageStatusInfoAsync(cacheProvider, cacheKey, statusInfo);
        }

        private async Task UpdateMessageStatusInfoAsync(INSCacheProvider cacheProvider, string cacheKey, MessageStatusInfo statusInfo)
        {
            await cacheProvider.SetOrUpdateAsync(cacheKey, statusInfo, TimeSpan.FromDays(BLConsts.MESSAGE_STATUS_CACHE_TTL_IN_DAYS));
        }

        class GradeClassInfo
        {
            public string Name { get; set; }
            public bool IsImprovement { get; set; }

            public GradeClassInfo(string name, bool isImprovement)
            {
                Name = name;
                IsImprovement = isImprovement;
            }
        }
    }
}
