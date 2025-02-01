namespace NS.Quizzy.Server.DAL.Entities
{
    public class Questionnaire : BaseEntity
    {
        public uint Code { get; set; }
        public string Name { get; set; }
        public Guid SubjectId { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan DurationWithExtra { get; set; }


        public virtual Subject Subject { get; set; }
        public virtual IList<Exam> Exams { get; set; }
    }
}
