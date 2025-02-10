using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NS.Quizzy.Server.Common.Extensions;
using NS.Shared.CacheProvider.Interfaces;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.HostedServices
{
    internal class AppLifetimeInfoHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public AppLifetimeInfoHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var cacheProvider = scope.ServiceProvider.GetRequiredService<INSCacheProvider>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var configKey = AppSettingKeys.ServerInfoTTLMin.GetDBStringValue();
            var serverInfoTTLMin = double.TryParse(config.GetValue<string>(configKey), out double ttlMin) ? ttlMin : 20160; //2 Week
            await cacheProvider.SetOrUpdateAsync($"ServerInfo:{Environment.MachineName}:StartTime", DateTimeOffset.Now, TimeSpan.FromMinutes(serverInfoTTLMin));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var cacheProvider = scope.ServiceProvider.GetRequiredService<INSCacheProvider>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var configKey = AppSettingKeys.ServerInfoTTLMin.GetDBStringValue();
            var serverInfoTTLMin = double.TryParse(config.GetValue<string>(configKey), out double ttlMin) ? ttlMin : 20160; //2 Week
            await cacheProvider.SetOrUpdateAsync($"ServerInfo:{Environment.MachineName}:StopTime", DateTimeOffset.Now, TimeSpan.FromMinutes(serverInfoTTLMin));
        }
    }
}
