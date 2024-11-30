namespace NS.Quizzy.Server.Models.DTOs
{
    public class BaseEntityDto
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset ModifiedTime { get; set; }

        public bool IsDeleted { get; set; }
    }
}
