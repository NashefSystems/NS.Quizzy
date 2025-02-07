using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.Extensions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Utils;
using NS.Quizzy.Server.DAL;
using NS.Shared.Logging;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.Services
{
    internal class AppSettingsService : IAppSettingsService
    {
        private readonly INSLogger _logger;
        private readonly AppDbContext _dbContext;
        private const string APP_VERSION_KEY = "AppVersion";

        public AppSettingsService(INSLogger logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<object?> GetByKeyAsync(string key)
        {
            var item = await _dbContext.AppSettings.FirstOrDefaultAsync(x => x.Key == key);
            return item.GetValueByType(_logger);
        }

        public async Task<Dictionary<string, object?>> GetByTargetAsync(AppSettingTargets target)
        {
            var items = await _dbContext.AppSettings.Where(x => x.Target == target || x.Target == AppSettingTargets.All).ToListAsync();
            var res = items.ToDictionary(k => k.Key, v => v.GetValueByType(_logger));

            #region Set app version
            var appVersion = AppUtils.GetAppVersionAsString();
            if (!string.IsNullOrWhiteSpace(appVersion))
            {
                if (res.ContainsKey(APP_VERSION_KEY))
                {
                    res[APP_VERSION_KEY] = appVersion;
                }
                else
                {
                    res.Add(APP_VERSION_KEY, appVersion);
                }
            }
            #endregion

            return res;
        }
    }
}
