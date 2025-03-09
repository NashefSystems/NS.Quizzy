using Newtonsoft.Json;
using NS.Quizzy.Server.DAL;

namespace NS.Quizzy.Server.Models.DTOs
{
    public class UserBaseDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public DALEnums.Roles Role { get; set; }
        public Guid? ClassId { get; set; }
    }

    public class UserPayloadDto : UserBaseDto
    {
        public string Password { get; set; }
    }

    public class UserDto : UserBaseDto, IBaseEntityDto
    {
        [JsonProperty(Order = int.MinValue)]
        public Guid Id { get; set; }
    }
}
