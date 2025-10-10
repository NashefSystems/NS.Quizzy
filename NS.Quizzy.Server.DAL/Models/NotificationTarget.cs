using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.DAL.Models
{
    [NotMapped]
    public class NotificationTarget
    {
        public NotificationTargetTypes Type { get; set; }
        public List<Guid>? Ids { get; set; }
    }
}
