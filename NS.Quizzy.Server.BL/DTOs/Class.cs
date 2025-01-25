using Newtonsoft.Json;

namespace NS.Quizzy.Server.Models.DTOs
{
    public class ClassPayloadDto
    {
        public string Name { get; set; }
        public virtual List<ClassDto> Children { get; set; }
    }

    public class ClassDto : ClassPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
