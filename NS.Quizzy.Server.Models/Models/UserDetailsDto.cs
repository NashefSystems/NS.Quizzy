namespace NS.Quizzy.Server.Models.Models
{
    public class UserDetailsDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Guid TokenId { get; set; }
        public string Token { get; set; }
    }
}
