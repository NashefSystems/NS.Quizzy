using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NS.Quizzy.Server.BL.Extensions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.Services
{
    internal class AppSettingsService : IAppSettingsService
    {
        private readonly INSLogger _logger;
        private readonly AppDbContext _dbContext;

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
            return items.ToDictionary(k => k.Key, v => v.GetValueByType(_logger));
        }
    }
}
