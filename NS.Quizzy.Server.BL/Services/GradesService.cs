using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.Services
{
    internal class GradesService : IGradesService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public GradesService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<List<GradeDto>> GetAllAsync()
        {
            var items = await _appDbContext.Grades
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Code)
                .ToListAsync();
            return _mapper.Map<List<GradeDto>>(items);
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
                throw new BadRequestException("Grade already exists");
            }

            var item = new DAL.Entities.Grade()
            {
                Code = model.Code,
                Name = model.Name,
            };
            await _appDbContext.Grades.AddAsync(item);
            await _appDbContext.SaveChangesAsync();

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
                throw new BadRequestException("There is another grade with the same name");
            }

            item.Code = model.Code;
            item.Name = model.Name;
            await _appDbContext.SaveChangesAsync();

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
            return true;
        }
    }
}
