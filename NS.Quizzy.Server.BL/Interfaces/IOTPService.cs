using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Interfaces
{
    internal interface IOTPService
    {
        string GenerateQRCode(string secretKey, string email);
        string GenerateSecretKey();
        bool VerifyOTP(string twoFactorSecretKey, string oTP);
    }
}
