using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NS.Quizzy.Server.Common.Extensions;
using NS.Shared.CacheProvider.Interfaces;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.AppConfiguration
{
    internal class DbConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly IServiceProvider _rootServiceProvider;
        private readonly Timer _reloadTimer;
        private readonly TimeSpan _reloadInterval = TimeSpan.FromMinutes(10);
        
        private const string _query = "SELECT [Key], [Value] FROM [AppSettings] WHERE [IsDeleted] = 0 AND [Target] != 'Client';"; // Except Client

        public DbConfigurationProvider(IServiceProvider rootServiceProvider)
        {
            _rootServiceProvider = rootServiceProvider;
            Load();  // Initial Load

            _reloadTimer = new Timer(_ => Reload(), null, _reloadInterval, _reloadInterval);
        }

        public override void Load()
        {
            try
            {
                using var scope = _rootServiceProvider.CreateScope();
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var settings = new Dictionary<string, string?>();
                using var connection = new SqlConnection(config.GetConnectionString("QuizzyDB"));
                using var query = new SqlCommand(_query, connection);
                query.Connection.Open();
                using var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    var key = reader[0]?.ToString();
                    if (!string.IsNullOrEmpty(key))
                    {
                        settings.Add(key, reader[1].ToString());
                    }
                }

                var cacheKey = AppSettingKeys.ServerInfoTTLMin.GetDBStringValue();

                var serverInfoTTLMin = settings.TryGetValue(cacheKey, out var ttlMinStr) && double.TryParse(ttlMinStr, out double ttlMin) ?
                ttlMin : 10080; //1 Week

                var cacheProvider = scope.ServiceProvider.GetRequiredService<INSCacheProvider>();
                cacheProvider.SetOrUpdateAsync($"ServerInfo:{Environment.MachineName}:LastLoadAppSettingsTime", DateTimeOffset.Now, TimeSpan.FromMinutes(serverInfoTTLMin));

                Data = settings;
            }
            catch (Exception) { }
            Data ??= new Dictionary<string, string?>();
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