namespace NS.Quizzy.Server.DAL.Entities
{
    public class LoginHistoryItem : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string? UserAgent { get; set; }
        public string? ClientIP { get; set; }
        public string? Platform { get; set; }
        public bool IsMobile { get; set; }
        public string? Country { get; set; }

        public virtual User User { get; set; }
    }
}
