using Newtonsoft.Json;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class MoedPayloadDto
    {
        public string Name { get; set; }
        public int ItemOrder { get; set; }
    }

    public class MoedDto : MoedPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
