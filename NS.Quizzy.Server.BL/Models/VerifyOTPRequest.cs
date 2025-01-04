using System.ComponentModel.DataAnnotations;

namespace NS.Quizzy.Server.BL.Models
{
    public class VerifyOTPRequest
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
