namespace NS.Quizzy.Server.Models.DTOs
{
    public class QuestionnaireDto : BaseEntityDto
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public Guid SubjectId { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan DurationWithExtra { get; set; }


        public virtual SubjectDto Subject { get; set; }
        public virtual IList<ExamDto> Exams { get; set; }

    }
}
