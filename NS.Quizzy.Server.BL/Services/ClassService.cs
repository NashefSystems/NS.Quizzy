using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ClassService : IClassService
    {
        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClassDto>> GetAllAsync()
        {
            throw new NotImplementedException();
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
