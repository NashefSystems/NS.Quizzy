using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.BL.DTOs;
using NS.Shared.CacheProvider.Interfaces;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.Services
{
    internal class MoedsService : IMoedsService
    {
        const string CACHE_KEY = "Moeds";
        private readonly INSCacheProvider _cacheProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly TimeSpan _cacheDataTTL;

        public MoedsService(AppDbContext appDbContext, INSCacheProvider cacheProvider, IMapper mapper, IConfiguration configuration)
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

        public async Task<List<MoedDto>> GetAllAsync()
        {
            var cacheValue = await _cacheProvider.GetAsync<List<MoedDto>>(CACHE_KEY);
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var items = await _appDbContext.Moeds
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.ItemOrder)
                .ThenBy(x => x.Name)
                .ToListAsync();
            var res = _mapper.Map<List<MoedDto>>(items);
            await _cacheProvider.SetOrUpdateAsync(CACHE_KEY, res, _cacheDataTTL);
            return res;
        }

        public async Task<MoedDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Moeds.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            return _mapper.Map<MoedDto>(item);
        }

        public async Task<MoedDto> InsertAsync(MoedPayloadDto model)
        {
            var exists = await _appDbContext.Moeds.AnyAsync(x => x.IsDeleted == false && x.Name == model.Name);
            if (exists)
            {
                throw new ConflictException("Moed already exists");
            }

            var item = new DAL.Entities.Moed()
            {
                Name = model.Name,
                ItemOrder = model.ItemOrder
            };
            await _appDbContext.Moeds.AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<MoedDto>(item);
        }

        public async Task<MoedDto?> UpdateAsync(Guid id, MoedPayloadDto model)
        {
            var item = await _appDbContext.Moeds.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            if (await _appDbContext.Moeds.AnyAsync(x => x.IsDeleted == false && x.Id != id && x.Name == model.Name))
            {
                throw new ConflictException("There is another moed with the same name");
            }

            item.Name = model.Name;
            item.ItemOrder = model.ItemOrder;
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<MoedDto>(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.Moeds.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
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
