using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
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
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public ExamsService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ExamDto>> FilterAsync(DateTimeOffset? dtFrom, DateTimeOffset? dtTo, List<Guid>? classIds, List<Guid>? subjectIds)
        {
            dtFrom ??= new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtTo ??= dtFrom.Value.AddMonths(1).AddMicroseconds(-1);

            var query = _appDbContext.Exams.Where(x => x.StartTime >= dtFrom.Value && x.StartTime <= dtTo.Value);

            if (classIds?.Any() ?? false)
            {
                query = query
                    .Include(x => x.ClassExams)
                    .Where(x => x.ClassExams.Any(y => classIds.Contains(y.ClassId)));
            }

            if (subjectIds?.Any() ?? false)
            {
                query = query
                    .Include(x => x.Questionnaire)
                    .Where(x => subjectIds.Contains(x.Questionnaire.SubjectId));
            }

            var data = await query.ToListAsync();
            return _mapper.Map<List<ExamDto>>(data);
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
