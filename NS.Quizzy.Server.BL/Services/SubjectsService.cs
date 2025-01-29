using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.Services
{
    internal class SubjectsService : ISubjectsService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public SubjectsService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<List<SubjectDto>> GetAllAsync()
        {
            var items = await _appDbContext.Subjects
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.ItemOrder)
                .ThenBy(x => x.Name)
                .ToListAsync();
            return _mapper.Map<List<SubjectDto>>(items);
        }

        public async Task<SubjectDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Subjects.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            return _mapper.Map<SubjectDto>(item);
        }

        public async Task<SubjectDto> InsertAsync(SubjectPayloadDto model)
        {
            var exists = await _appDbContext.Subjects.AnyAsync(x => x.IsDeleted == false && x.Name == model.Name);
            if (exists)
            {
                throw new BadRequestException("Subject already exists");
            }

            var item = new DAL.Entities.Subject()
            {
                Name = model.Name,
                ItemOrder = model.ItemOrder
            };
            await _appDbContext.Subjects.AddAsync(item);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<SubjectDto>(item);
        }

        public async Task<SubjectDto?> UpdateAsync(Guid id, SubjectPayloadDto model)
        {
            var item = await _appDbContext.Subjects.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            if (await _appDbContext.Subjects.AnyAsync(x => x.IsDeleted == false && x.Id != id && x.Name == model.Name))
            {
                throw new BadRequestException("There is another subject with the same name");
            }

            item.Name = model.Name;
            item.ItemOrder = model.ItemOrder;
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<SubjectDto>(item);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.Subjects.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
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
