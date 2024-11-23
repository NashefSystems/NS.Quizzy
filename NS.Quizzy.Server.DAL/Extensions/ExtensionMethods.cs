using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NS.Quizzy.Server.DAL.Extensions
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(
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
