namespace NS.Quizzy.Server.Models.DTOs
{
    public class ClassDto : BaseEntityDto
    {
        public string Name { get; set; }
        public Guid? ParentId { get; set; }

        public virtual ClassDto Parent { get; set; }
        public virtual IList<ClassDto> Children { get; set; }
        public virtual IList<ClassExamDto> ClassExams { get; set; }
    }
}
