using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.BL.DTOs;
using NS.Shared.CacheProvider.Interfaces;
using static NS.Quizzy.Server.Common.Enums;
using NS.Quizzy.Server.BL.Interfaces;

namespace NS.Quizzy.Server.BL.Services
{
    internal class NotificationGroupsService : INotificationGroupsService
    {
        const string CACHE_KEY = "NotificationGroups";
        private readonly INSCacheProvider _cacheProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly TimeSpan _cacheDataTTL;

        public NotificationGroupsService(AppDbContext appDbContext, INSCacheProvider cacheProvider, IMapper mapper, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _cacheProvider = cacheProvider;
            _mapper = mapper;
            {
                var configKey = AppSettingKeys.CacheDataTTLMin.GetDBStringValue();
                var valueInMin = double.TryParse(configuration.GetValue<string>(configKey), out double val) ? val : 60;
                _cacheDataTTL = TimeSpan.FromMinutes(valueInMin);
            }
        }

        public async Task<List<NotificationGroupDto>> GetAllAsync()
        {
            var cacheValue = await _cacheProvider.GetAsync<List<NotificationGroupDto>>(CACHE_KEY);
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var items = await _appDbContext.NotificationGroups
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Name)
                .ToListAsync();
            var res = _mapper.Map<List<NotificationGroupDto>>(items);
            await _cacheProvider.SetOrUpdateAsync(CACHE_KEY, res, _cacheDataTTL);
            return res;
        }

        public async Task<NotificationGroupDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.NotificationGroups.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            return _mapper.Map<NotificationGroupDto>(item);
        }

        public async Task<NotificationGroupDto> InsertAsync(NotificationGroupPayloadDto model)
        {
            if (await _appDbContext.NotificationGroups.AnyAsync(x => x.IsDeleted == false && x.Name == model.Name.Trim()))
            {
                throw new ConflictException("NotificationGroup already exists");
            }

            var item = new DAL.Entities.NotificationGroup()
            {
                Name = model.Name.Trim(),
                UserIds = model.UserIds,
            };
            await _appDbContext.NotificationGroups.AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<NotificationGroupDto>(item);
        }

        public async Task<NotificationGroupDto?> UpdateAsync(Guid id, NotificationGroupPayloadDto model)
        {
            var item = await _appDbContext.NotificationGroups.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            if (await _appDbContext.NotificationGroups.AnyAsync(x => x.IsDeleted == false && x.Id != id && x.Name == model.Name.Trim()))
            {
                throw new ConflictException("There is another questionnaire with the same code");
            }

            item.Name = model.Name.Trim();
            item.UserIds = model.UserIds;
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<NotificationGroupDto>(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.NotificationGroups.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return false;
            }

            item.IsDeleted = true;
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);
            return true;
        }
    }
}
