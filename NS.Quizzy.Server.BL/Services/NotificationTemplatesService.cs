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
    internal class NotificationTemplatesService : INotificationTemplatesService
    {
        const string CACHE_KEY = "DBCache:NotificationTemplates";
        private readonly INSCacheProvider _cacheProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly TimeSpan _cacheDataTTL;

        public NotificationTemplatesService(AppDbContext appDbContext, INSCacheProvider cacheProvider, IMapper mapper, IConfiguration configuration)
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

        public async Task<List<NotificationTemplateDto>> GetAllAsync()
        {
            var cacheValue = await _cacheProvider.GetAsync<List<NotificationTemplateDto>>(CACHE_KEY);
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var items = await _appDbContext.NotificationTemplates
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Name)
                .ToListAsync();
            var res = _mapper.Map<List<NotificationTemplateDto>>(items);
            await _cacheProvider.SetOrUpdateAsync(CACHE_KEY, res, _cacheDataTTL);
            return res;
        }

        public async Task<NotificationTemplateDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.NotificationTemplates.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            return _mapper.Map<NotificationTemplateDto>(item);
        }

        public async Task<NotificationTemplateDto> InsertAsync(NotificationTemplatePayloadDto model)
        {
            if (await _appDbContext.NotificationTemplates.AnyAsync(x => x.IsDeleted == false && x.Name == model.Name.Trim()))
            {
                throw new ConflictException("NotificationTemplate already exists");
            }

            var item = new DAL.Entities.NotificationTemplate()
            {
                Name = model.Name.Trim(),
                Title = model.Title,
                Body = model.Body,
            };
            await _appDbContext.NotificationTemplates.AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<NotificationTemplateDto>(item);
        }

        public async Task<NotificationTemplateDto?> UpdateAsync(Guid id, NotificationTemplatePayloadDto model)
        {
            var item = await _appDbContext.NotificationTemplates.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            if (await _appDbContext.NotificationTemplates.AnyAsync(x => x.IsDeleted == false && x.Id != id && x.Name == model.Name.Trim()))
            {
                throw new ConflictException("There is another notification template with the same name");
            }

            item.Name = model.Name.Trim();
            item.Title = model.Title;
            item.Body = model.Body;

            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<NotificationTemplateDto>(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.NotificationTemplates.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
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
