using Newtonsoft.Json;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class GradeExamPayloadDto
    {
        public Guid GradeId { get; set; }
        public Guid ExamId { get; set; }
    }

    public class GradeExamDto : IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }

        public virtual GradeDto Grade { get; set; }
        public virtual ExamDto Exam { get; set; }
    }
}
