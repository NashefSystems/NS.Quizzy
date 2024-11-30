namespace NS.Quizzy.Server.Models.DTOs
{
    public class ExamTypeDto : BaseEntityDto
    {
        public string Name { get; set; }
        public int ItemOrder { get; set; }
    }
}
