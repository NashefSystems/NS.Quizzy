using NS.Shared.Logging;
using NS.Shared.Logging.Attributes;
using System.ComponentModel.DataAnnotations;

namespace NS.Quizzy.Server.Models.Models
{
    public class LoginWithIdNumberRequest
    {
        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        public string IdNumber { get; set; }
    }
}
