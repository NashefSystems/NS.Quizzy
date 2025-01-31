using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.Models.DTOs;

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

        public async Task<List<ExamDto>> FilterAsync(DateTimeOffset? dtFrom, DateTimeOffset? dtTo, List<Guid>? classIds, List<Guid>? subjectIds)
        {
            dtFrom ??= new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtTo ??= dtFrom.Value.AddMonths(3).AddMicroseconds(-1);

            var query = _appDbContext.Exams.Where(x => x.IsDeleted == false && x.StartTime >= dtFrom.Value && x.StartTime <= dtTo.Value);

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

            var data = await query.OrderBy(x => x.StartTime).ToListAsync();
            return _mapper.Map<List<ExamDto>>(data);
        }

        public async Task<List<ExamDto>> GetAllAsync()
        {
            return await FilterAsync(null, null, null, null);
        }

        public async Task<ExamDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Exams.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }
            var res = _mapper.Map<ExamDto>(item);

            res.ClassIds = await _appDbContext.ClassExams
                .Where(x => x.IsDeleted == false && x.ExamId == item.Id)
                .Select(x => x.ClassId)
                .ToListAsync();

            res.GradeIds = await _appDbContext.GradeExams
             .Where(x => x.IsDeleted == false && x.ExamId == item.Id)
             .Select(x => x.GradeId)
             .ToListAsync();

            return res;
        }

        public async Task<ExamDto> InsertAsync(ExamPayloadDto model)
        {
            model.ClassIds ??= [];
            model.GradeIds ??= [];

            await ValidationMethod(model);

            var exam = new Exam()
            {
                Duration = model.Duration,
                DurationWithExtra = model.DurationWithExtra,
                ExamTypeId = model.ExamTypeId,
                QuestionnaireId = model.QuestionnaireId,
                StartTime = model.StartTime,
            };
            await _appDbContext.Exams.AddAsync(exam);
            await _appDbContext.SaveChangesAsync();
            foreach (var classId in model.ClassIds)
            {
                await _appDbContext.ClassExams.AddAsync(new()
                {
                    ClassId = classId,
                    ExamId = exam.Id,
                });
            }
            foreach (var gradeId in model.GradeIds)
            {
                await _appDbContext.GradeExams.AddAsync(new()
                {
                    GradeId = gradeId,
                    ExamId = exam.Id,
                });
            }
            await _appDbContext.SaveChangesAsync();
            var res = _mapper.Map<ExamDto>(exam);
            res.ClassIds = model.ClassIds;
            res.GradeIds = model.GradeIds;
            return res;
        }

        public async Task<ExamDto?> UpdateAsync(Guid id, ExamPayloadDto model)
        {
            var item = await _appDbContext.Exams.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }

            model.ClassIds ??= [];
            model.GradeIds ??= [];

            await ValidationMethod(model);

            item.Duration = model.Duration;
            item.DurationWithExtra = model.DurationWithExtra;
            item.ExamTypeId = model.ExamTypeId;
            item.QuestionnaireId = model.QuestionnaireId;
            item.StartTime = model.StartTime;
            await _appDbContext.SaveChangesAsync();

            {
                var dbClassExams = await _appDbContext.ClassExams.Where(x => x.IsDeleted == false && x.ExamId == item.Id).ToListAsync();
                var dbClassIds = dbClassExams.Select(x => x.ClassId).ToList();

                var dbClassIdsToDelete = dbClassIds.Where(x => !model.ClassIds.Contains(x)).ToList();
                if (dbClassIdsToDelete.Count > 0)
                {
                    var itemsToRemove = dbClassExams.Where(x => dbClassIdsToDelete.Contains(x.ClassId)).ToList();
                    //_appDbContext.ClassExams.RemoveRange(itemsToRemove);
                    itemsToRemove.ForEach(x => x.IsDeleted = true);
                }

                var classIdsToAdd = model.ClassIds.Where(x => !dbClassIds.Contains(x)).ToList();
                foreach (var classId in classIdsToAdd)
                {
                    await _appDbContext.ClassExams.AddAsync(new()
                    {
                        ClassId = classId,
                        ExamId = item.Id,
                    });
                }
            }

            {
                var dbGradeExams = await _appDbContext.GradeExams.Where(x => x.IsDeleted == false && x.ExamId == item.Id).ToListAsync();
                var dbGradeIds = dbGradeExams.Select(x => x.GradeId).ToList();

                var dbGradeIdsToDelete = dbGradeIds.Where(x => !model.GradeIds.Contains(x)).ToList();
                if (dbGradeIdsToDelete.Count > 0)
                {
                    var itemsToRemove = dbGradeExams.Where(x => dbGradeIdsToDelete.Contains(x.GradeId)).ToList();
                    _appDbContext.GradeExams.RemoveRange(itemsToRemove);
                }

                var gradeIdsToAdd = model.GradeIds.Where(x => !dbGradeIds.Contains(x)).ToList();
                foreach (var gradeId in gradeIdsToAdd)
                {
                    await _appDbContext.GradeExams.AddAsync(new()
                    {
                        GradeId = gradeId,
                        ExamId = item.Id,
                    });
                }
            }

            await _appDbContext.SaveChangesAsync();
            var res = _mapper.Map<ExamDto>(item);
            res.ClassIds = model.ClassIds;
            res.GradeIds = model.GradeIds;
            return res;
        }

        private async Task ValidationMethod(ExamPayloadDto model)
        {
            var examTypeIsExists = await _appDbContext.ExamTypes.AnyAsync(x => x.IsDeleted == false && x.Id == model.ExamTypeId);
            if (!examTypeIsExists)
            {
                throw new BadRequestException("Exam type ID not found");
            }

            var questionnaireIsExists = await _appDbContext.Questionnaires.AnyAsync(x => x.IsDeleted == false && x.Id == model.QuestionnaireId);
            if (!questionnaireIsExists)
            {
                throw new BadRequestException("Questionnaire ID not found");
            }

            var dbClassIds = await _appDbContext.Classes.Where(x => x.IsDeleted == false).Select(x => x.Id).ToListAsync();
            var invalidClassIds = model.ClassIds.Where(x => !dbClassIds.Contains(x)).ToList();
            if (invalidClassIds.Count != 0)
            {
                throw new BadRequestException($"Invalid class ID's [{string.Join(", ", invalidClassIds)}]");
            }

            var dbGradeIds = await _appDbContext.Grades.Where(x => x.IsDeleted == false).Select(x => x.Id).ToListAsync();
            var invalidGradeIds = model.GradeIds.Where(x => !dbGradeIds.Contains(x)).ToList();
            if (invalidGradeIds.Count != 0)
            {
                throw new BadRequestException($"Invalid grade ID's [{string.Join(", ", invalidGradeIds)}]");
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.Exams.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return false;
            }

            item.IsDeleted = true;
            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
