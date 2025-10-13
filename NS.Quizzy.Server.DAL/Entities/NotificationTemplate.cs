namespace NS.Quizzy.Server.DAL.Entities
{
    public class NotificationTemplate : BaseEntity
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
