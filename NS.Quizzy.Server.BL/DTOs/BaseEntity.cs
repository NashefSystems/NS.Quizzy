using Newtonsoft.Json;

namespace NS.Quizzy.Server.Models.DTOs
{
    public class BaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
