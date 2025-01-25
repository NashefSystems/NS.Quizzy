using Newtonsoft.Json;

namespace NS.Quizzy.Server.Models.DTOs
{
    public interface IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
