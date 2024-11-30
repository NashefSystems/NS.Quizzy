using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ClassDto>> GetAllAsync()
        {
            var items = await _appDbContext.Classes
                .Where(x => x.IsDeleted == false && x.ParentId == null)
                .Include(x => x.Children)
                .OrderBy(x => x.Name)
                .ToListAsync();
            return _mapper.Map<List<ClassDto>>(items);
        }

        public Task<ClassDto> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ClassDto> InsertAsync(ClassDto model)
        {
            throw new NotImplementedException();
        }

        public Task<ClassDto> UpdateAsync(Guid id, ClassDto model)
        {
            throw new NotImplementedException();
        }
    }
}
