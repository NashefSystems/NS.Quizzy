using Newtonsoft.Json;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class ClassExamPayloadDto
    {
        public Guid ClassId { get; set; }
        public Guid ExamId { get; set; }
    }

    public class ClassExamDto : IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }

        public virtual ClassDto Class { get; set; }
        public virtual ExamDto Exam { get; set; }
    }
}
