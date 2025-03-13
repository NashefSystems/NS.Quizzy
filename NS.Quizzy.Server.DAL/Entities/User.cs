using NS.Shared.Logging;
using NS.Shared.Logging.Attributes;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        [SensitiveData(SensitiveDataMode.Sha1)]
        public string Password { get; set; }
        public string FullName { get; set; }
        public Guid? ClassId { get; set; }
        public Roles Role { get; set; }
        public string? TwoFactorSecretKey { get; set; }

        public virtual Class? Class { get; set; }
        public virtual IList<LoginHistoryItem> LoginHistory { get; set; }
    }
}
