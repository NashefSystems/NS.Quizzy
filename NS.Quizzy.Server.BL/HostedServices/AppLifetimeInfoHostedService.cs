using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NS.Quizzy.Server.Common.Extensions;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var cacheKey = AppSettingKeys.ServerInfoTTLMin.GetDBStringValue();
            var serverInfoTTLMin = double.TryParse(config.GetValue<string>(cacheKey), out double ttlMin) ? ttlMin : 10080; //1 Week
            await cacheProvider.SetOrUpdateAsync($"ServerInfo:{Environment.MachineName}:StartTime", DateTimeOffset.Now, TimeSpan.FromMinutes(serverInfoTTLMin));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var cacheProvider = scope.ServiceProvider.GetRequiredService<INSCacheProvider>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var cacheKey = AppSettingKeys.ServerInfoTTLMin.GetDBStringValue();
            var serverInfoTTLMin = double.TryParse(config.GetValue<string>(cacheKey), out double ttlMin) ? ttlMin : 10080; //1 Week
            await cacheProvider.SetOrUpdateAsync($"ServerInfo:{Environment.MachineName}:StopTime", DateTimeOffset.Now, TimeSpan.FromMinutes(serverInfoTTLMin));
        }
    }
}
