using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.CacheProvider.models;
using NS.Shared.Logging;
using System.Text;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ExamEvents : IExamEvents
    {
        private readonly IZohoCalendarService _zohoCalendarService;
        private readonly ILockManager _lockManager;
        private readonly AppDbContext _appDbContext;

        public ExamEvents(AppDbContext appDbContext, IZohoCalendarService zohoCalendarService, ILockManager lockManager)
        {
            _appDbContext = appDbContext;
            _zohoCalendarService = zohoCalendarService;
            _lockManager = lockManager;
        }

        public async Task ResyncEventAsync(Guid examId, INSLogBag logBag)
        {
            var lockKey = $"lock:{nameof(ResyncEventAsync)}";
            AcquireLockResponse? acquireLockResponse = null;
            try
            {
                acquireLockResponse = await _lockManager.AcquireLockAsync(lockKey, TimeSpan.FromHours(1));
                var exam = await _appDbContext.Exams.FirstOrDefaultAsync(x => x.Id == examId)
                    ?? throw new Exception($"Exam ID '{examId}' not found");

                var emails = await _appDbContext.EventEmails.Where(x => x.IsDeleted == false).Select(x => x.Email).ToListAsync();
                logBag.AddOrUpdateParameter("ExamIsDeleted", exam.IsDeleted);
                logBag.AddOrUpdateParameter("ExamIsVisible", exam.IsVisible);
                logBag.AddOrUpdateParameter("ExamCalendarEventId", exam.CalendarEventId);


                if (exam.IsVisible && !exam.IsDeleted)
                {
                    logBag.Trace("Create or update event");

                    var classes = (await _appDbContext.ClassExams
                        .Include(x => x.Class)
                        .Where(x => x.IsDeleted == false && x.ExamId == exam.Id && x.Class.IsDeleted == false)
                        .ToListAsync())
                        .Select(x => new GradeClassInfo(x.Class.Name, x.IsImprovement))
                        .OrderBy(x => x.Name)
                        .ToList();

                    var grades = (await _appDbContext.GradeExams
                        .Include(x => x.Grade)
                        .Where(x => x.IsDeleted == false && x.ExamId == exam.Id && x.Grade.IsDeleted == false)
                        .ToListAsync())
                        .Select(x => new GradeClassInfo(x.Grade.Name, x.IsImprovement))
                        .OrderBy(x => x.Name)
                        .ToList();

                    var location = string.Join(", ", grades.Union(classes).Select(x => x.Name).ToList());
                    var examType = await _appDbContext.ExamTypes.FirstOrDefaultAsync(x => x.Id == exam.ExamTypeId);
                    var moed = await _appDbContext.Moeds.FirstOrDefaultAsync(x => x.Id == exam.MoedId);
                    var questionnaire = await _appDbContext.Questionnaires.FirstOrDefaultAsync(x => x.Id == exam.QuestionnaireId);
                    var subject = questionnaire?.SubjectId == null ? null : await _appDbContext.Subjects.FirstOrDefaultAsync(x => x.Id == questionnaire.SubjectId);
                    var body = GetBody(exam, moed, examType, questionnaire, subject, grades, classes);
                    var isAllDay = exam.StartTime.Hour == 0 && exam.StartTime.Minute == 0;
                    var endTime = isAllDay ? exam.StartTime.DateTime.AddDays(1) : exam.StartTime.DateTime.Add(exam.DurationWithExtra);
                    var calendarEvent = new ZohoCalendarEvent()
                    {
                        Title = $"{subject?.Name} ({questionnaire?.Code}) - {examType?.Name} - {exam.Duration:hh\\:mm} / {exam.DurationWithExtra:hh\\:mm}",
                        Location = location,
                        Description = body,
                        StartTime = exam.StartTime.DateTime,
                        EndTime = endTime,
                        IsAllDay = isAllDay,
                        Emails = emails
                    };

                    logBag.Trace(string.IsNullOrWhiteSpace(exam.CalendarEventId) ? "Create" : "Update" + " event");
                    exam.CalendarEventId = await _zohoCalendarService.CreateOrUpdateEventAsync(calendarEvent, exam.CalendarEventId);
                    logBag.Trace($"CalendarEventId: {exam.CalendarEventId}");
                    logBag.AddOrUpdateParameter("ExamCalendarEventId", exam.CalendarEventId);
                }
                else if (!string.IsNullOrWhiteSpace(exam.CalendarEventId))
                {
                    logBag.Trace("Delete event");
                    await _zohoCalendarService.DeleteEventAsync(exam.CalendarEventId);
                    exam.CalendarEventId = null;
                }
                await _appDbContext.SaveChangesAsync();
                logBag.Trace("ResyncEventAsync completed");
            }
            finally
            {
                if (acquireLockResponse?.Token != null)
                {
                    await _lockManager.ReleaseLockAsync(lockKey, acquireLockResponse.Token);
                }
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
