using Newtonsoft.Json;
using NS.Quizzy.Server.DAL;

namespace NS.Quizzy.Server.BL.DTOs
{
    public class UserPayloadDto
    {
        public string Email { get; set; }
        public string? IdNumber { get; set; }
        public string FullName { get; set; }
        public DALEnums.Roles Role { get; set; }
        public Guid? ClassId { get; set; }
    }

    public class UserDto : UserPayloadDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
