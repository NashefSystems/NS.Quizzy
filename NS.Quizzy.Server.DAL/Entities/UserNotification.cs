namespace NS.Quizzy.Server.DAL.Entities
{
    public class UserNotification : BaseEntity
    {
        public Guid NotificationId { get; set; }
        public virtual Notification Notification { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public DateTimeOffset? SeenAt { get; set; }
    }
}
