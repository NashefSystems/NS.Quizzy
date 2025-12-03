namespace NS.Quizzy.Server.BL.Models
{
    public class ReSyncEventsResponse
    {
        public int Total { get; set; }
        public List<Guid> ExamIds { get; set; }
    }
}
