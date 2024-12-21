using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Services
{
    internal class SubjectsService : ISubjectsService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public SubjectsService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SubjectDto>> GetAllAsync()
        {
            var items = await _appDbContext.Subjects
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.ItemOrder)
                .ThenBy(x => x.Name)
                .ToListAsync();
            return _mapper.Map<List<SubjectDto>>(items);
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
