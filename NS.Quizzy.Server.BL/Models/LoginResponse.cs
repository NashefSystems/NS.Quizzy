namespace NS.Quizzy.Server.Models.Models
{
    public class LoginResponse
    {
        public bool RequiresTwoFactor { get; set; }
        public string? RequestId { get; set; }
        public string? QrCode { get; set; }
        public string? TwoFactorSecretKey { get; set; }
    }
}
