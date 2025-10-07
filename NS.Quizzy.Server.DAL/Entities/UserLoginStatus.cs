namespace NS.Quizzy.Server.DAL.Entities
{
    public class UserLoginStatus
    {
        public required string IdNumber { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }
        public uint? Class { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public string? IsAllowNotifications { get; set; }
    }
}
