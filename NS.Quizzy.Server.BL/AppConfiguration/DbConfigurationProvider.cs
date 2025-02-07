using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Shared.CacheProvider.Interfaces;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.AppConfiguration
{
    internal class DbConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly IServiceProvider _rootServiceProvider;
        private readonly Timer _reloadTimer;
        private readonly TimeSpan _reloadInterval = TimeSpan.FromMinutes(10);

        public DbConfigurationProvider(IServiceProvider rootServiceProvider)
        {
            _rootServiceProvider = rootServiceProvider;
            Load();  // Initial Load

            _reloadTimer = new Timer(_ => Reload(), null, _reloadInterval, _reloadInterval);
        }

        public override void Load()
        {
            using var scope = _rootServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cacheProvider = scope.ServiceProvider.GetRequiredService<INSCacheProvider>();

            var settings = dbContext.AppSettings
                .Where(x => x.IsDeleted == false && x.Target != DALEnums.AppSettingTargets.Client)
                .ToDictionary(k => k.Key, v => v.Value);
            Data = settings;

            var cacheKey = AppSettingKeys.ServerInfoTTLMin.GetDBStringValue();
            var serverInfoTTLMin = settings.TryGetValue(cacheKey, out var ttlMinStr) && double.TryParse(ttlMinStr, out double ttlMin) ?
            ttlMin : 10080; //1 Week
            cacheProvider.SetOrUpdateAsync($"ServerInfo:{Environment.MachineName}:LastLoadAppSettingsTime", DateTimeOffset.Now, TimeSpan.FromMinutes(serverInfoTTLMin));
        }

        private void Reload()
        {
            Load();      // Reload data
            OnReload();  // Notify IConfiguration of changes
        }

        public void Dispose()
        {
            _reloadTimer?.Dispose();
        }
    }
}