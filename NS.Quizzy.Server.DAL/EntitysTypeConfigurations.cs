using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.DAL.Attributes;
using NS.Quizzy.Server.DAL.Entities;
using NS.Security;
using System.Reflection;
using static NS.Quizzy.Server.DAL.DALEnums;
using NS.Quizzy.Server.DAL.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NS.Quizzy.Server.DAL
{
    internal static class EntitysTypeConfigurations
    {
        private static readonly SecurityLogic _securityLogic = new("678rfrgf789plkfmk_NS.Quizzy.Server.DAL_890kjnfdd66");

        internal static void SetConfigurations(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ExamEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ClassEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ClassExamEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GradeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new GradeExamEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ExamTypeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MoedEntityConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionnaireEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectEntityConfiguration());
            modelBuilder.ApplyConfiguration(new AppSettingEntityConfiguration());
            modelBuilder.ApplyConfiguration(new LoginHistoryItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserNotificationEntityConfiguration());

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var entity = modelBuilder.Entity(entityType.ClrType);
                int columnOrder = 1;
                foreach (var property in entityType.GetProperties())
                {
                    var columnOrderAttribute = property.PropertyInfo?.GetCustomAttribute<DBColumnOrderAttribute>();
                    entity.Property(property.Name).HasColumnOrder(columnOrderAttribute?.Order ?? columnOrder++);
                }
            }
        }

        internal abstract class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
        {
            public virtual void Configure(EntityTypeBuilder<TEntity> entity)
            {
                entity
                      .HasKey(x => x.Id);
                entity
                    .Property(x => x.Id)
                    .HasColumnType("uniqueidentifier")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(NewId())");

                entity
                    .Property(p => p.CreatedTime)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(SYSDATETIMEOFFSET())");
                entity
                    .Property(p => p.ModifiedTime)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(SYSDATETIMEOFFSET())");
                entity
                    .Property(p => p.IsDeleted)
                    .HasDefaultValue(false);
            }
        }

        internal class UserEntityConfiguration : BaseEntityTypeConfiguration<User>
        {
            public override void Configure(EntityTypeBuilder<User> entity)
            {
                base.Configure(entity);
                entity.ToTable("Users");

                entity.HasIndex(p => p.Email).IsUnique(true).HasFilter("IsDeleted = '0'");
                entity.Property(e => e.Email).IsRequired(true);

                entity
                    .Property(e => e.Password)
                    .HasConversion(v => UsersPasswordToDBValue(v), dbv => UsersPasswordFromDBValue(dbv));

                entity
                   .Property(e => e.Role)
                   .IsRequired(true)
                   .HasDefaultValue(Roles.Student);

                entity.Property(e => e.TwoFactorSecretKey).HasMaxLength(50);

                entity.Property(e => e.FullName).IsRequired(true);

                entity
                .HasOne(c => c.Class)
                .WithMany(c => c.Users)
                .HasForeignKey(c => c.ClassId);

                entity.HasData(InitialData.UserEntityData.GetData());
            }

            private static string UsersPasswordFromDBValue(string dbValue)
            {
                if (string.IsNullOrWhiteSpace(dbValue))
                {
                    return string.Empty;
                }
                return _securityLogic.Decrypt(dbValue);
            }

            private static string UsersPasswordToDBValue(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return string.Empty;
                }
                return _securityLogic.Encrypt(value);
            }
        }

        internal class ExamEntityConfiguration : BaseEntityTypeConfiguration<Exam>
        {
            public override void Configure(EntityTypeBuilder<Exam> entity)
            {
                base.Configure(entity);
                entity.ToTable("Exams");

                entity.Property(x => x.MoedId).IsRequired(true);

                entity
                    .HasOne(c => c.Questionnaire)
                    .WithMany(c => c.Exams)
                    .HasForeignKey(c => c.QuestionnaireId);

                entity
                  .HasOne(c => c.ExamType)
                  .WithMany(c => c.Exams)
                  .HasForeignKey(c => c.ExamTypeId);

                entity
                  .HasOne(c => c.Moed)
                  .WithMany(c => c.Exams)
                  .HasForeignKey(c => c.MoedId);
            }
        }

        internal class ClassEntityConfiguration : BaseEntityTypeConfiguration<Class>
        {
            public override void Configure(EntityTypeBuilder<Class> entity)
            {
                base.Configure(entity);
                entity.ToTable("Classes");

                entity.HasIndex(p => new { p.Name, p.GradeId }).IsUnique(true).HasFilter("IsDeleted = '0'");
                entity
                    .HasOne(c => c.Grade)
                    .WithMany(c => c.Classes)
                    .HasForeignKey(c => c.GradeId);

                entity.HasData(InitialData.ClassEntityData.GetData());
            }
        }

        internal class ClassExamEntityConfiguration : BaseEntityTypeConfiguration<ClassExam>
        {
            public override void Configure(EntityTypeBuilder<ClassExam> entity)
            {
                base.Configure(entity);
                entity.ToTable("ClassExams");

                entity
                    .HasOne(c => c.Exam)
                    .WithMany(c => c.ClassExams)
                    .HasForeignKey(c => c.ExamId);

                entity
                    .HasOne(c => c.Class)
                    .WithMany(c => c.ClassExams)
                    .HasForeignKey(c => c.ClassId);
            }
        }

        internal class GradeEntityConfiguration : BaseEntityTypeConfiguration<Grade>
        {
            public override void Configure(EntityTypeBuilder<Grade> entity)
            {
                base.Configure(entity);
                entity.ToTable("Grades");

                entity.HasIndex(p => p.Name).IsUnique(true).HasFilter("IsDeleted = '0'");
                entity.HasData(InitialData.GradeEntityData.GetData());
            }
        }

        internal class GradeExamEntityConfiguration : BaseEntityTypeConfiguration<GradeExam>
        {
            public override void Configure(EntityTypeBuilder<GradeExam> entity)
            {
                base.Configure(entity);
                entity.ToTable("GradeExams");

                entity
                    .HasOne(c => c.Exam)
                    .WithMany(c => c.GradeExams)
                    .HasForeignKey(c => c.ExamId);

                entity
                    .HasOne(c => c.Grade)
                    .WithMany(c => c.GradeExams)
                    .HasForeignKey(c => c.GradeId);
            }
        }

        internal class ExamTypeEntityConfiguration : BaseEntityTypeConfiguration<ExamType>
        {
            public override void Configure(EntityTypeBuilder<ExamType> entity)
            {
                base.Configure(entity);
                entity.ToTable("ExamTypes");
                entity.HasIndex(p => p.Name).IsUnique(true).HasFilter("IsDeleted = '0'");
                entity.Property(p => p.Name).IsRequired(true);

                entity.HasData(InitialData.ExamTypeEntityData.GetData());
            }
        }

        internal class MoedEntityConfiguration : BaseEntityTypeConfiguration<Moed>
        {
            public override void Configure(EntityTypeBuilder<Moed> entity)
            {
                base.Configure(entity);
                entity.ToTable("Moeds");
                entity.HasIndex(p => p.Name).IsUnique(true).HasFilter("IsDeleted = '0'");
                entity.Property(p => p.Name).IsRequired(true);

                entity.HasData(InitialData.MoedEntityData.GetData());
            }
        }

        internal class QuestionnaireEntityConfiguration : BaseEntityTypeConfiguration<Questionnaire>
        {
            public override void Configure(EntityTypeBuilder<Questionnaire> entity)
            {
                base.Configure(entity);
                entity.ToTable("Questionnaires");
                entity.HasIndex(p => p.Code).IsUnique(true).HasFilter("IsDeleted = '0'");

                entity
                  .HasOne(c => c.Subject)
                  .WithMany(c => c.Questionnaires)
                  .HasForeignKey(c => c.SubjectId);

            }
        }

        internal class SubjectEntityConfiguration : BaseEntityTypeConfiguration<Subject>
        {
            public override void Configure(EntityTypeBuilder<Subject> entity)
            {
                base.Configure(entity);
                entity.ToTable("Subjects");
                entity.HasIndex(p => p.Name).IsUnique(true).HasFilter("IsDeleted = '0'");

                entity.HasData(InitialData.SubjectEntityData.GetData());

            }
        }

        internal class AppSettingEntityConfiguration : BaseEntityTypeConfiguration<AppSetting>
        {
            public override void Configure(EntityTypeBuilder<AppSetting> entity)
            {
                base.Configure(entity);
                entity.ToTable("AppSettings");
                entity.HasIndex(p => p.Key).IsUnique(true).HasFilter("IsDeleted = '0'");

                entity
                    .Property(e => e.ValueType)
                    .IsRequired(true)
                    .HasConversion(v => v.ToStringValue(), dbv => dbv.ToEnumValue<AppSettingValueTypes>());

                entity
                    .Property(e => e.Target)
                    .IsRequired(true)
                    .HasConversion(v => v.ToStringValue(), dbv => dbv.ToEnumValue<AppSettingTargets>());

                entity.HasData(InitialData.AppSettingEntityData.GetData());
            }
        }

        internal class LoginHistoryItemEntityConfiguration : BaseEntityTypeConfiguration<LoginHistoryItem>
        {
            public override void Configure(EntityTypeBuilder<LoginHistoryItem> entity)
            {
                base.Configure(entity);
                entity.ToTable("LoginHistory");
                entity.HasIndex(p => p.UserId).HasFilter("IsDeleted = '0'");

                entity
                  .HasOne(c => c.User)
                  .WithMany(c => c.LoginHistory)
                  .HasForeignKey(c => c.UserId);

            }
        }

        internal class NotificationEntityConfiguration : BaseEntityTypeConfiguration<Notification>
        {
            public override void Configure(EntityTypeBuilder<Notification> entity)
            {
                base.Configure(entity);
                entity.ToTable("Notifications");

                entity
                    .Property(e => e.Data)
                    .HasColumnName("DataJson")
                    .HasConversion(v => ObjectToJson(v), dbv => JsonToObject<Dictionary<string, string>>(dbv));

                entity
                    .Property(e => e.TargetIds)
                    .HasColumnName("TargetIdsJson")
                    .HasConversion(v => ObjectToJson(v), dbv => JsonToObject<List<Guid>>(dbv));

                entity
                  .HasOne(c => c.CreatedBy)
                  .WithMany(c => c.NotificationsICreated)
                  .HasForeignKey(c => c.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);
            }

            private static T? JsonToObject<T>(string dbValue)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(dbValue))
                    {
                        return JsonConvert.DeserializeObject<T>(dbValue);
                    }
                }
                catch (Exception) { }
                return default;
            }

            private static string ObjectToJson<T>(T value)
            {
                return JsonConvert.SerializeObject(value);
            }
        }

        internal class UserNotificationEntityConfiguration : BaseEntityTypeConfiguration<UserNotification>
        {
            public override void Configure(EntityTypeBuilder<UserNotification> entity)
            {
                base.Configure(entity);
                entity.ToTable("UserNotifications");

                entity
                    .HasOne(c => c.Notification)
                    .WithMany(c => c.UserNotifications)
                    .HasForeignKey(c => c.NotificationId);

                entity
                    .HasOne(c => c.User)
                    .WithMany(c => c.UserNotifications)
                    .HasForeignKey(c => c.UserId);
            }
        }
    }
}
