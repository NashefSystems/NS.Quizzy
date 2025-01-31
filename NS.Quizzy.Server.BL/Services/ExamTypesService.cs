using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ExamTypesService : IExamTypesService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ExamTypesService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<List<ExamTypeDto>> GetAllAsync()
        {
            var items = await _appDbContext.ExamTypes
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.ItemOrder)
                .ThenBy(x => x.Name)
                .ToListAsync();
            return _mapper.Map<List<ExamTypeDto>>(items);
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
            return true;
        }
    }
}
