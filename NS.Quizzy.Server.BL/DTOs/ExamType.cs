using Newtonsoft.Json;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class ExamTypePayloadDto
    {
        public string Name { get; set; }
        public int ItemOrder { get; set; }
    }

    public class ExamTypeDto : ExamTypePayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
