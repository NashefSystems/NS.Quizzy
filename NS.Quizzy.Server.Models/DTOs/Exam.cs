namespace NS.Quizzy.Server.Models.DTOs
{
    public class ExamDto : BaseEntityDto
    {
        public Guid QuestionnaireId { get; set; }
        public Guid ExamTypeId { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan DurationWithExtra { get; set; }
      

        public virtual QuestionnaireDto Questionnaire { get; set; }
        public virtual ExamTypeDto ExamType { get; set; }
        public virtual IList<ClassExamDto> ClassExams { get; set; }
    }
}
