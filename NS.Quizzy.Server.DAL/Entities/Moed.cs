namespace NS.Quizzy.Server.DAL.Entities
{
    public class Moed : BaseEntity
    {
        public string Name { get; set; }
        public int ItemOrder { get; set; }

        public virtual IList<Exam> Exams { get; set; }
    }
}
