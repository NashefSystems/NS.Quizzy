using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public NotificationTarget Target { get; set; }
        public string? P1 { get; set; }
        public string? P2 { get; set; }
        public string? P3 { get; set; }

        public Guid CreatedBy { get; set; }
        public virtual IList<UserNotification> UserNotifications { get; set; }
    }
}
