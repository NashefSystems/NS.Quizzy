using Newtonsoft.Json;

namespace NS.Quizzy.Server.Models.DTOs
{
    public class ExamPayloadDto
    {
        public Guid QuestionnaireId { get; set; }
        public Guid ExamTypeId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan DurationWithExtra { get; set; }
    }

    public class ExamDto : ExamPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }

        public virtual QuestionnaireDto Questionnaire { get; set; }
        public virtual ExamTypeDto ExamType { get; set; }
        public virtual IList<ClassExamDto> ClassExams { get; set; }
    }
}
