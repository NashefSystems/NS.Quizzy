using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.Models.DTOs;
using NS.Quizzy.Server.Models.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IExamsService : IBaseService<ExamPayloadDto, ExamDto>
    {
        Task<List<ExamDto>> FilterAsync(ExamFilterRequest request);
        Task<List<ExamDto>> GetAllAsync(bool filterCompletedExams);
    }
}
