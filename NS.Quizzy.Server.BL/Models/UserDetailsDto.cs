using Newtonsoft.Json;
using NS.Quizzy.Server.DAL;

namespace NS.Quizzy.Server.Models.Models
{
    public class UserDetailsDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }
        public string IdNumber { get; set; }

        public string Email { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Guid? TokenId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        public DALEnums.Roles Role { get; set; }
        public Guid? ClassId { get; set; }
        public bool PushNotificationIsEnabled { get; set; }
    }
}
