using Microsoft.Extensions.Configuration;

namespace NS.Quizzy.Server.BL.AppConfiguration
{
    internal class DbConfigurationSource : IConfigurationSource
    {
        private readonly IServiceProvider _rootServiceProvider;

        public DbConfigurationSource(IServiceProvider rootServiceProvider)
        {
            _rootServiceProvider = rootServiceProvider;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DbConfigurationProvider(_rootServiceProvider);
        }
    }
}
