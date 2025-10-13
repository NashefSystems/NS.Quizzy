using NS.Quizzy.Server.DAL.Models;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public List<NotificationTarget> Targets { get; set; }
        public Dictionary<string, string>? Data { get; set; }
        public Guid CreatedById { get; set; }

        public virtual User CreatedBy { get; set; }
        public virtual IList<UserNotification> UserNotifications { get; set; }
    }
}
