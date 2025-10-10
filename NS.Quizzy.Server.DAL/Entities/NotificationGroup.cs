using NS.Quizzy.Server.DAL.Models;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class NotificationGroup : BaseEntity
    {
        public string Name { get; set; }
        public List<Guid> UserIds { get; set; }
    }
}
