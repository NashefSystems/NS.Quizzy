namespace NS.Quizzy.Server.DAL.Entities
{
    public class Class : BaseEntity
    {
        public Guid GradeId { get; set; }
        public uint Code { get; set; }
        public string Name { get; set; }

        public virtual Grade Grade { get; set; }
        public virtual IList<ClassExam> ClassExams { get; set; }
        public virtual IList<User> Users { get; set; }
    }
}
