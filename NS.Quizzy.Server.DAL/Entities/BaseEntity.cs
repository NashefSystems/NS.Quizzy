using NS.Quizzy.Server.DAL.Attributes;

namespace NS.Quizzy.Server.DAL.Entities
{
    public class BaseEntity
    {
        [DBColumnOrder(int.MinValue)]
        public Guid Id { get; set; }

        [DBColumnOrder(int.MaxValue - 2)]
        public DateTimeOffset CreatedTime { get; set; }

        [DBColumnOrder(int.MaxValue - 1)]
        public DateTimeOffset ModifiedTime { get; set; }

        [DBColumnOrder(int.MaxValue)]
        public bool IsDeleted { get; set; }

        public override int GetHashCode()
        {
            return $"{GetType()}_{Id}".GetHashCode();
        }
    }
}
