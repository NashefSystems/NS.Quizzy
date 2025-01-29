using AutoMapper;
using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.Services
{
    internal class QuestionnairesService : IQuestionnairesService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public QuestionnairesService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<List<QuestionnaireDto>> GetAllAsync()
        {
            var items = await _appDbContext.Questionnaires
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Code)
                .ToListAsync();
            return _mapper.Map<List<QuestionnaireDto>>(items);
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
                throw new BadRequestException("Questionnaire already exists");
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
                throw new BadRequestException("There is another questionnaire with the same code");
            }

            item.Code = model.Code;
            item.Name = model.Name;
            item.SubjectId = model.SubjectId;
            item.Duration = model.Duration;
            item.DurationWithExtra = model.DurationWithExtra;
            await _appDbContext.SaveChangesAsync();

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
            return true;
        }
    }
}
