using Newtonsoft.Json;

namespace NS.Quizzy.Server.Models.DTOs
{
    public class SubjectPayloadDto
    {
        public string Name { get; set; }
        public int ItemOrder { get; set; }
    }

    public class SubjectDto : SubjectPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
