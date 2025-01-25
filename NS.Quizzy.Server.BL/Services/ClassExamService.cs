using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ClassExamService : IClassExamService
    {
        public Task<List<ClassExamDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ClassExamDto?> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ClassExamDto> InsertAsync(ClassExamPayloadDto model)
        {
            throw new NotImplementedException();
        }

        public Task<ClassExamDto?> UpdateAsync(Guid id, ClassExamPayloadDto model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
