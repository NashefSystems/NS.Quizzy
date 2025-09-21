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
    internal class GradesService : IGradesService
    {
        const string CACHE_KEY = "Grades";
        private readonly INSCacheProvider _cacheProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly TimeSpan _cacheDataTTL;

        public GradesService(AppDbContext appDbContext, INSCacheProvider cacheProvider, IMapper mapper, IConfiguration configuration)
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

        public async Task<List<GradeDto>> GetAllAsync()
        {
            var cacheValue = await _cacheProvider.GetAsync<List<GradeDto>>(CACHE_KEY);
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var items = await _appDbContext.Grades
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Code)
                .ToListAsync();
            var res = _mapper.Map<List<GradeDto>>(items);
            await _cacheProvider.SetOrUpdateAsync(CACHE_KEY, res, _cacheDataTTL);
            return res;
        }

        public async Task<GradeDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Grades.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            return _mapper.Map<GradeDto>(item);
        }

        public async Task<GradeDto> InsertAsync(GradePayloadDto model)
        {
            var exists = await _appDbContext.Grades.AnyAsync(x => x.IsDeleted == false && x.Name == model.Name);
            if (exists)
            {
                throw new ConflictException("Grade already exists");
            }

            var item = new DAL.Entities.Grade()
            {
                Code = model.Code,
                Name = model.Name,
            };
            await _appDbContext.Grades.AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<GradeDto>(item);
        }

        public async Task<GradeDto?> UpdateAsync(Guid id, GradePayloadDto model)
        {
            var item = await _appDbContext.Grades.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            if (await _appDbContext.Grades.AnyAsync(x => x.IsDeleted == false && x.Id != id && x.Name == model.Name))
            {
                throw new ConflictException("There is another grade with the same name");
            }

            item.Code = model.Code;
            item.Name = model.Name;
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<GradeDto>(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.Grades.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
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
