using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.BL.DTOs;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IExamsService : IBaseService<ExamPayloadDto, ExamDto>
    {
        Task<List<ExamDto>> FilterAsync(ExamFilterRequest request);
        Task<List<ExamDto>> GetAllAsync(bool filterCompletedExams);
        Task<ExamDto?> SetAsVisibleAsync(Guid id);
    }
}
