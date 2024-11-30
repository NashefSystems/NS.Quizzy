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
    }
}
