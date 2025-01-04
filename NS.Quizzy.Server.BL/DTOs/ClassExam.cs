namespace NS.Quizzy.Server.Models.DTOs
{
    public class ClassExamDto : BaseEntityDto
    {
        public Guid ClassId { get; set; }
        public virtual ClassDto Class { get; set; }

        public Guid ExamId { get; set; }
        public virtual ExamDto Exam { get; set; }
    }
}
