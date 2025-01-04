using NS.Shared.Logging;
using NS.Shared.Logging.Attributes;
using System.ComponentModel.DataAnnotations;

namespace NS.Quizzy.Server.Models.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [SensitiveData(SensitiveDataMode.Sha1)]
        public string Password { get; set; }
    }
}
