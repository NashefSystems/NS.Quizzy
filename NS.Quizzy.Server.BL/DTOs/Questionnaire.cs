using Newtonsoft.Json;

namespace NS.Quizzy.Server.Models.DTOs
{
    public class QuestionnairePayloadDto
    {
        public uint Code { get; set; }
        public string Name { get; set; }
        public Guid SubjectId { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan DurationWithExtra { get; set; }
    }

    public class QuestionnaireDto : QuestionnairePayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
        public virtual SubjectDto Subject { get; set; }
        public virtual IList<ExamDto> Exams { get; set; }
    }
}
