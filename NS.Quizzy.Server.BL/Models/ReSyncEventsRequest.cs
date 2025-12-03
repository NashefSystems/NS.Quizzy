namespace NS.Quizzy.Server.BL.Models
{
    public class ReSyncEventsRequest
    {
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public List<Guid>? ExamIds { get; set; }
    }
}
