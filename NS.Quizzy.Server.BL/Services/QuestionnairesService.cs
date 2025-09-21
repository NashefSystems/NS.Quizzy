using AutoMapper;
using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.BL.DTOs;
using NS.Shared.CacheProvider.Interfaces;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.Services
{
    internal class QuestionnairesService : IQuestionnairesService
    {
        const string CACHE_KEY = "Questionnaires";
        private readonly INSCacheProvider _cacheProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly TimeSpan _cacheDataTTL;

        public QuestionnairesService(AppDbContext appDbContext, INSCacheProvider cacheProvider, IMapper mapper, IConfiguration configuration)
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

        public async Task<List<QuestionnaireDto>> GetAllAsync()
        {
            var cacheValue = await _cacheProvider.GetAsync<List<QuestionnaireDto>>(CACHE_KEY);
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var items = await _appDbContext.Questionnaires
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Code)
                .ToListAsync();
            var res = _mapper.Map<List<QuestionnaireDto>>(items);
            await _cacheProvider.SetOrUpdateAsync(CACHE_KEY, res, _cacheDataTTL);
            return res;
        }

        public async Task<QuestionnaireDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Questionnaires.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            return _mapper.Map<QuestionnaireDto>(item);
        }

        public async Task<QuestionnaireDto> InsertAsync(QuestionnairePayloadDto model)
        {
            if (await _appDbContext.Questionnaires.AnyAsync(x => x.IsDeleted == false && x.Code == model.Code))
            {
                throw new ConflictException("Questionnaire already exists");
            }

            var item = new DAL.Entities.Questionnaire()
            {
                Code = model.Code,
                Name = model.Name,
                SubjectId = model.SubjectId,
                Duration = model.Duration,
                DurationWithExtra = model.DurationWithExtra,
            };
            await _appDbContext.Questionnaires.AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<QuestionnaireDto>(item);
        }

        public async Task<QuestionnaireDto?> UpdateAsync(Guid id, QuestionnairePayloadDto model)
        {
            var item = await _appDbContext.Questionnaires.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            if (await _appDbContext.Questionnaires.AnyAsync(x => x.IsDeleted == false && x.Id != id && x.Code == model.Code))
            {
                throw new ConflictException("There is another questionnaire with the same code");
            }

            item.Code = model.Code;
            item.Name = model.Name;
            item.SubjectId = model.SubjectId;
            item.Duration = model.Duration;
            item.DurationWithExtra = model.DurationWithExtra;
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<QuestionnaireDto>(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.Questionnaires.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
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
