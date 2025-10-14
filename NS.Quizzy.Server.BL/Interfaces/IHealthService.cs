
using NS.Quizzy.Server.BL.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IHealthService
    {
        Task<ReadinessResult> ReadinessAsync();
    }
}
