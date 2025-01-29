using Newtonsoft.Json;

namespace NS.Quizzy.Server.Models.DTOs
{
    public class GradePayloadDto
    {
        public string Name { get; set; }
        public uint Code { get; set; }
    }

    public class GradeDto : GradePayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }

        public List<ClassDto> Classes { get; set; }
    }
}
