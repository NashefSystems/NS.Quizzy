using NS.Shared.Logging;

namespace NS.Quizzy.Server.BL.Interfaces
{
    internal interface IExamEvents
    {
        Task ResyncEventAsync(Guid examId, INSLogBag logBag);
    }
}
