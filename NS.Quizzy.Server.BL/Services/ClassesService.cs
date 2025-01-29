using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ClassesService : IClassesService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ClassesService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<List<ClassDto>> GetAllAsync()
        {
            var grades = await _appDbContext.Grades
                .AsNoTracking()
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Code)
                .ToDictionaryAsync(k => k.Id, v => v.Code);

            var items = await _appDbContext.Classes
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.Code)
                .ToListAsync();

            return items
                .Select(x => new ClassDto()
                {
                    Code = x.Code,
                    Name = x.Name,
                    Id = x.Id,
                    GradeId = x.GradeId,
                    FullCode = GetFullCode(grades, x.GradeId, x.Code),
                })
                .OrderBy(x => x.FullCode)
                .ToList();
        }

        private uint GetFullCode(Dictionary<Guid, uint> gradeCodes, Guid gradeId, uint classCode)
        {
            var res = classCode;
            if (gradeCodes.TryGetValue(gradeId, out uint value))
            {
                res += value * 100;
            }
            return res;
        }

        public async Task<ClassDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Classes.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            var gradeDic = await _appDbContext.Grades
                .AsNoTracking()
                .Where(x => x.Id == item.GradeId && x.IsDeleted == false)
                .ToDictionaryAsync(k => k.Id, v => v.Code);

            return new ClassDto()
            {
                Code = item.Code,
                Name = item.Name,
                Id = item.Id,
                GradeId = item.GradeId,
                FullCode = GetFullCode(gradeDic, item.GradeId, item.Code),
            };
        }

        public async Task<ClassDto> InsertAsync(ClassPayloadDto model)
        {
            var exists = await _appDbContext.Classes.AnyAsync(x => x.IsDeleted == false && x.Name == model.Name && x.GradeId == model.GradeId);
            if (exists)
            {
                throw new BadRequestException("Class already exists");
            }

            var gradeDic = await _appDbContext.Grades
                .AsNoTracking()
               .Where(x => x.Id == model.GradeId && x.IsDeleted == false)
               .ToDictionaryAsync(k => k.Id, v => v.Code);

            if (gradeDic.Count() == 0)
            {
                throw new BadRequestException("Grade not found");
            }

            var item = new DAL.Entities.Class()
            {
                GradeId = model.GradeId,
                Name = model.Name,
                Code = model.Code,
            };

            await _appDbContext.Classes.AddAsync(item);
            await _appDbContext.SaveChangesAsync();

            return new ClassDto()
            {
                Code = item.Code,
                Name = item.Name,
                Id = item.Id,
                GradeId = item.GradeId,
                FullCode = GetFullCode(gradeDic, item.GradeId, item.Code),
            };
        }

        public async Task<ClassDto?> UpdateAsync(Guid id, ClassPayloadDto model)
        {
            var item = await _appDbContext.Classes.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            var gradeDic = await _appDbContext.Grades
                .AsNoTracking()
                .Where(x => x.Id == model.GradeId && x.IsDeleted == false)
                .ToDictionaryAsync(k => k.Id, v => v.Code);

            if (gradeDic.Count() == 0)
            {
                throw new BadRequestException("Grade not found");
            }

            item.Name = model.Name;
            item.GradeId = model.GradeId;
            item.Code = model.Code;
            await _appDbContext.SaveChangesAsync();

            return new ClassDto()
            {
                Code = item.Code,
                Name = item.Name,
                Id = item.Id,
                GradeId = item.GradeId,
                FullCode = GetFullCode(gradeDic, item.GradeId, item.Code),
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.Classes.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
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
