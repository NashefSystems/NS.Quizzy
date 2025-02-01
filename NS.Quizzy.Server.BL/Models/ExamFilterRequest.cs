namespace NS.Quizzy.Server.BL.Models
{
    public class ExamFilterRequest
    {
        public DateTimeOffset FromTime { get; set; }
        public DateTimeOffset ToTime { get; set; }
        public List<Guid>? ExamTypeIds { get; set; }
        public List<Guid>? MoedIds { get; set; }
        public List<Guid>? QuestionnaireIds { get; set; }
        public List<Guid>? GradeIds { get; set; }
        public List<Guid>? ClassIds { get; set; }
        public List<Guid>? SubjectIds { get; set; }
    }
}
