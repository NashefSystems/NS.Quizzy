using Newtonsoft.Json;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class ClassPayloadDto
    {
        public Guid GradeId { get; set; }
        public string Name { get; set; }
        public uint Code { get; set; }
    }

    public class ClassDto : ClassPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
        public uint FullCode { get; set; }
    }
}
