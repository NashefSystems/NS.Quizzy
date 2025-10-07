namespace NS.Quizzy.Server.BL.DTOs
{
    public class UserLoginStatusDto
    {
        public required string IdNumber { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }
        public uint? Class { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public string? IsAllowNotifications { get; set; }
    }
}
