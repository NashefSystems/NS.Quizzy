namespace NS.Quizzy.Server.DAL.Entities
{
    public class Subject : BaseEntity
    {
        public string Name { get; set; }
        public int ItemOrder { get; set; }

        public virtual IList<Questionnaire> Questionnaires { get; set; }

    }
}
