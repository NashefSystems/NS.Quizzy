using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Interfaces
{
    internal interface IOTPService
    {
        string GetTwoFactorUrl(string secretKey, string email);
        string GenerateQRCode(string url);
        string GenerateSecretKey();
        bool VerifyOTP(string twoFactorSecretKey, string oTP);
    }
}
