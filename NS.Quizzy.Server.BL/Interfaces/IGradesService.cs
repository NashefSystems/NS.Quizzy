using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IGradesService : IBaseService<GradePayloadDto, GradeDto>
    {
    }
}
