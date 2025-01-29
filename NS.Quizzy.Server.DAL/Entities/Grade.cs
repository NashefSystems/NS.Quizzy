namespace NS.Quizzy.Server.DAL.Entities
{
    public class Grade : BaseEntity
    {
        public uint Code { get; set; }
        public string Name { get; set; }
        public virtual IList<Class> Classes { get; set; }
        public virtual IList<GradeExam> GradeExams { get; set; }
    }
}
