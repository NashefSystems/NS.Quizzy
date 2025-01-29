using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;

namespace NS.Quizzy.Server.BL.Services
{
    internal class TestService : ITestService
    {
        private readonly AppDbContext _appDbContext;

        public TestService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<object> TestAsync()
        {
            Random rnd = new Random();

            var questionnaire = new DAL.Entities.Questionnaire()
            {
                Code = (uint)(10000 + rnd.Next(8888)),
                Name = "ערבית למתקדמים",
                SubjectId = Guid.Parse("9B8F8B4E-19FE-4F98-A9E1-0234D8331F24"),
                Duration = TimeSpan.FromMinutes(30),
                DurationWithExtra = TimeSpan.FromMinutes(45),
            };
            await _appDbContext.Questionnaires.AddAsync(questionnaire);
            await _appDbContext.SaveChangesAsync();

            var exam = new DAL.Entities.Exam()
            {
                QuestionnaireId = questionnaire.Id,
                ExamTypeId = Guid.Parse("FA73C215-82D0-4BCD-BE22-B0EABB950315"),
                StartTime = new DateTime(2025, 1, 2, 10, 0, 0),
                Duration = TimeSpan.FromMinutes(60),
                DurationWithExtra = TimeSpan.FromMinutes(70),
            };
            await _appDbContext.Exams.AddAsync(exam);
            await _appDbContext.SaveChangesAsync();
            return exam;
        }
    }
}
