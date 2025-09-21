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
    internal class ExamTypesService : IExamTypesService
    {
        const string CACHE_KEY = "ExamTypes";
        private readonly INSCacheProvider _cacheProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly TimeSpan _cacheDataTTL;

        public ExamTypesService(AppDbContext appDbContext, INSCacheProvider cacheProvider, IMapper mapper, IConfiguration configuration)
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

        public async Task<List<ExamTypeDto>> GetAllAsync()
        {
            var cacheValue = await _cacheProvider.GetAsync<List<ExamTypeDto>>(CACHE_KEY);
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var items = await _appDbContext.ExamTypes
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.ItemOrder)
                .ThenBy(x => x.Name)
                .ToListAsync();
            var res = _mapper.Map<List<ExamTypeDto>>(items);
            await _cacheProvider.SetOrUpdateAsync(CACHE_KEY, res, _cacheDataTTL);
            return res;
        }

        public async Task<ExamTypeDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.ExamTypes.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            return _mapper.Map<ExamTypeDto>(item);
        }

        public async Task<ExamTypeDto> InsertAsync(ExamTypePayloadDto model)
        {
            var exists = await _appDbContext.ExamTypes.AnyAsync(x => x.IsDeleted == false && x.Name == model.Name);
            if (exists)
            {
                throw new ConflictException("Exam type already exists");
            }

            var item = new DAL.Entities.ExamType()
            {
                Name = model.Name,
                ItemOrder = model.ItemOrder
            };
            await _appDbContext.ExamTypes.AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<ExamTypeDto>(item);
        }

        public async Task<ExamTypeDto?> UpdateAsync(Guid id, ExamTypePayloadDto model)
        {
            var item = await _appDbContext.ExamTypes.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            if (await _appDbContext.ExamTypes.AnyAsync(x => x.IsDeleted == false && x.Id != id && x.Name == model.Name))
            {
                throw new ConflictException("There is another exam type with the same name");
            }

            item.Name = model.Name;
            item.ItemOrder = model.ItemOrder;
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<ExamTypeDto>(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.ExamTypes.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
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
