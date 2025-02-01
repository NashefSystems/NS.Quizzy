using Newtonsoft.Json;

namespace NS.Quizzy.Server.Models.DTOs
{
    public class ExamPayloadDto
    {
        public DateTimeOffset StartTime { get; set; }
        public Guid QuestionnaireId { get; set; }
        public Guid ExamTypeId { get; set; }
        public Guid MoedId { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan DurationWithExtra { get; set; }
        public List<Guid>? GradeIds { get; set; }
        public List<Guid>? ClassIds { get; set; }
    }

    public class ExamDto : ExamPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
