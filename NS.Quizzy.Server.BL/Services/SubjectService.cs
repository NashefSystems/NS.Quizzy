using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Services
{
    internal class SubjectService : ISubjectService
    {
        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<SubjectDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SubjectDto> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<SubjectDto> InsertAsync(SubjectDto model)
        {
            throw new NotImplementedException();
        }

        public Task<SubjectDto> UpdateAsync(Guid id, SubjectDto model)
        {
            throw new NotImplementedException();
        }
    }
}
