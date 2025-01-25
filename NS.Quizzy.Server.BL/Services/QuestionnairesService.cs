using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Services
{
    internal class QuestionnairesService : IQuestionnairesService
    {
         public Task<List<QuestionnaireDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<QuestionnaireDto?> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionnaireDto> InsertAsync(QuestionnairePayloadDto model)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionnaireDto?> UpdateAsync(Guid id, QuestionnairePayloadDto model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
