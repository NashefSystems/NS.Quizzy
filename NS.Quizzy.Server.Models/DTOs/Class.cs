namespace NS.Quizzy.Server.Models.DTOs
{
    public class ClassDto : BaseEntityDto
    {
        public string Name { get; set; }
        public virtual List<ClassDto> Children { get; set; }
    }
}
