using NS.Shared.Logging;
using NS.Shared.Logging.Attributes;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        [SensitiveData(SensitiveDataMode.Sha1)]
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}
