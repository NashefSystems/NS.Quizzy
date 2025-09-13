using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Models
{
    internal class TwoFactorCacheInfo
    {
        public Guid UserId { get; set; }
        public string? TwoFactorSecretKey { get; set; }

        public string? DeviceId { get; set; }
        public string? AppVersion { get; set; }
        public string? NotificationToken { get; set; }
        public string? Platform { get; set; }
    }
}
