namespace NS.Quizzy.Server.DAL.Entities
{
    public class Exam : BaseEntity
    {
        public Guid QuestionnaireId { get; set; }
        public Guid ExamTypeId { get; set; }
        public Guid MoedId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan DurationWithExtra { get; set; }
        public bool IsVisible { get; set; }


        public virtual Questionnaire Questionnaire { get; set; }
        public virtual ExamType ExamType { get; set; }
        public virtual Moed? Moed { get; set; }
        public virtual IList<ClassExam> ClassExams { get; set; }
        public virtual IList<GradeExam> GradeExams { get; set; }
    }
}
