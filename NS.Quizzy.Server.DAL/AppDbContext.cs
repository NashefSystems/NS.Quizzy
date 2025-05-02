using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.DAL.Entities;


namespace NS.Quizzy.Server.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ExamType> ExamTypes { get; set; }
        public DbSet<Moed> Moeds { get; set; }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<GradeExam> GradeExams { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassExam> ClassExams { get; set; }
        public DbSet<LoginHistoryItem> LoginHistory { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<Device> Devices { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var startAction = DateTimeOffset.Now;
            foreach (var entry in this.ChangeTracker.Entries<BaseEntity>())
            {
                var entity = entry.Entity;
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entity.Id == Guid.Empty)
                        {
                            entity.Id = Guid.NewGuid();
                        }
                        entity.CreatedTime = startAction;
                        entity.ModifiedTime = startAction;
                        break;
                    case EntityState.Modified:
                        entity.ModifiedTime = startAction;
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.SetConfigurations();
            base.OnModelCreating(builder);
        }
    }
}
