namespace NS.Quizzy.Server.Models.Models
{
    public class LoginResponse
    {
        public bool RequiresTwoFactor { get; set; }
        public string? RequestId { get; set; }
        public string? TwoFactorUrl { get; set; }
        public string? TwoFactorQrCode { get; set; }
        public string? TwoFactorSecretKey { get; set; }
    }
}
