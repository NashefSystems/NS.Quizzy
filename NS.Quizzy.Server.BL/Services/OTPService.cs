using Microsoft.Extensions.Configuration;
using NS.Quizzy.Server.BL.Interfaces;
using OtpNet;
using QRCoder;

namespace NS.Quizzy.Server.BL.Services
{
    internal class OTPService : IOTPService
    {
        private readonly string _appName;

        public OTPService(IConfiguration configuration)
        {
            _appName = configuration.GetValue<string>("AppName") ?? "";
        }

        public string GetTwoFactorUrl(string secretKey, string email)
        {
            return $"otpauth://totp/{_appName}:{email}?secret={secretKey}&issuer={_appName}";
        }

        public string GenerateQRCode(string url)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return Convert.ToBase64String(qrCode.GetGraphic(20));
        }

        public string GenerateSecretKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public bool VerifyOTP(string twoFactorSecretKey, string otp)
        {
            var totp = new Totp(Base32Encoding.ToBytes(twoFactorSecretKey));
            return totp.VerifyTotp(otp, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
        }
    }
}
