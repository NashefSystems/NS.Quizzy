namespace NS.Quizzy.Server.Models.DTOs
{
    public class SubjectDto : BaseEntityDto
    {
        public string Name { get; set; }
        public int ItemOrder { get; set; }

        public virtual IList<QuestionnaireDto> Questionnaires { get; set; }

    }
}
