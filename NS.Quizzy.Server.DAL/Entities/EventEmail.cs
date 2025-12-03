using System.ComponentModel.DataAnnotations;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class EventEmail : BaseEntity
    {
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
