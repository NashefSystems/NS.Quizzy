using Microsoft.Extensions.Configuration;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Shared.CacheProvider.Interfaces;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.AppConfiguration
{
    internal class DbConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly INSCacheProvider _cacheProvider;
        private readonly Timer _reloadTimer;
        private readonly TimeSpan _reloadInterval = TimeSpan.FromMinutes(10);

        public DbConfigurationProvider(AppDbContext dbContext, INSCacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _dbContext = dbContext;
            Load();  // Initial Load

            _reloadTimer = new Timer(_ => Reload(), null, _reloadInterval, _reloadInterval);
        }

        public override void Load()
        {
            var settings = _dbContext.AppSettings
                .Where(x => x.IsDeleted == false && x.Target != DALEnums.AppSettingTargets.Client)
                .ToDictionary(k => k.Key, v => v.Value);
            Data = settings;

            var cacheKey = AppSettingKeys.ServerInfoTTLMin.GetDBStringValue();
            var serverInfoTTLMin = settings.TryGetValue(cacheKey, out var ttlMinStr) && double.TryParse(ttlMinStr, out double ttlMin) ?
            ttlMin : 10080; //1 Week
            _cacheProvider.SetOrUpdateAsync($"ServerInfo:{Environment.MachineName}:LastLoadAppSettingsTime", DateTime.Now, TimeSpan.FromMinutes(serverInfoTTLMin));
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