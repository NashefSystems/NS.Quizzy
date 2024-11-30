using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ExamTypesService : IExamTypesService
    {
        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExamTypeDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ExamTypeDto> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ExamTypeDto> InsertAsync(ExamTypeDto model)
        {
            throw new NotImplementedException();
        }

        public Task<ExamTypeDto> UpdateAsync(Guid id, ExamTypeDto model)
        {
            throw new NotImplementedException();
        }
    }
}
