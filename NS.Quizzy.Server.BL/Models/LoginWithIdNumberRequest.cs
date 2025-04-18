using System.ComponentModel.DataAnnotations;

namespace NS.Quizzy.Server.Models.Models
{
    public class LoginWithIdNumberRequest
    {
        [Required]
        public string IdNumber { get; set; }

        public string? DeviceId { get; set; }
        public string? NotificationToken { get; set; }
    }
}
