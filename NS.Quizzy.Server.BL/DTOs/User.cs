using Newtonsoft.Json;

namespace NS.Quizzy.Server.Models.DTOs
{
    public class UserPayloadDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }

    public class UserDto : UserPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
