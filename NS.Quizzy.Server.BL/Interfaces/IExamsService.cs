using NS.Quizzy.Server.Models.DTOs;
using NS.Quizzy.Server.Models.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IExamsService : IBaseService<ExamDto>
    {
        Task<List<ExamDto>> FilterAsync(DateTimeOffset? dtFrom, DateTimeOffset? dtTo, List<Guid>? classIds, List<Guid>? subjectIds);
    }
}
