using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ExamService : IExamService
    {
        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExamDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ExamDto> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ExamDto> InsertAsync(ExamDto model)
        {
            throw new NotImplementedException();
        }

        public Task<ExamDto> UpdateAsync(Guid id, ExamDto model)
        {
            throw new NotImplementedException();
        }
    }
}
