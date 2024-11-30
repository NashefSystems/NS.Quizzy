using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using NS.Quizzy.Server.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ExamsService : IExamsService
    {
        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExamDto>> FilterAsync(FilterRequest filter)
        {
            throw new NotImplementedException("Filter");
        }

        public Task<List<ExamDto>> GetAllAsync()
        {
            throw new NotImplementedException("Get all");
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
