using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NS.Shared.EFDBAudit.Extentions;

namespace NS.Quizzy.Server.DAL.Extensions
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddQuizzyDALServices(this IServiceCollection services)
        {
            services.AddDbContextWithNSDBAudit<AppDbContext>(
                (provider, options) =>
                {
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    var connectionString = configuration.GetConnectionString("QuizzyDB");
                    Console.WriteLine($"ConnectionString: '{connectionString}'");
                    options.UseSqlServer(connectionString);
                },
                ServiceLifetime.Scoped
            );
            return services;
        }

        public static T? ToEnumValue<T>(this string? dbValue)
        {
            if (string.IsNullOrWhiteSpace(dbValue))
            {
                return default;
            }
            return (T)Enum.Parse(typeof(T), dbValue);
        }

        public static string? ToStringValue<T>(this T? value) where T : Enum
        {
            if (value == null)
            {
                return null;
            }
            return value.ToString();
        }
    }
}


