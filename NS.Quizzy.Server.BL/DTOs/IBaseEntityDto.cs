using Newtonsoft.Json;

namespace NS.Quizzy.Server.BL.DTOs
{
    public interface IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
