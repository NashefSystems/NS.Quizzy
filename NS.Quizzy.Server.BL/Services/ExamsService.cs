using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.DTOs;
using NS.Quizzy.Server.BL.Extensions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Shared.Logging;
using NS.Shared.QueueManager.Interfaces;

namespace NS.Quizzy.Server.BL.Services
{
    internal class ExamsService : IExamsService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INSLogger _logger;
        private readonly INSQueueService _queueService;

        public ExamsService(AppDbContext appDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, INSLogger logger, INSQueueService queueService)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _queueService = queueService;
        }

        bool IsFilterHiddenExams()
        {
            var role = _httpContextAccessor.HttpContext.GetUserRole();
            if (role == null)
            {
                return true;
            }
            return role < DALEnums.Roles.Admin;
        }

        public async Task<List<ExamDto>> FilterAsync(ExamFilterRequest request)
        {
            using var logBag = _logger.CreateLogBag(nameof(FilterAsync));
            logBag.Trace("Starting");
            var query = _appDbContext.Exams
                .Include(x => x.GradeExams)
                .Include(x => x.ClassExams)
                .Where(x =>
                    x.IsDeleted == false &&
                    x.StartTime >= request.FromTime &&
                    x.StartTime <= request.ToTime
                );
            logBag.AddOrUpdateParameter(nameof(request), request);

            if (request.ExamTypeIds?.Count > 0)
            {
                logBag.Trace("Filter by ExamTypeIds");
                query = query
                    .Where(x => request.ExamTypeIds.Contains(x.ExamTypeId));
            }

            if (IsFilterHiddenExams())
            {
                logBag.Trace("Filter hidden exams");
                query = query
                  .Where(x => x.IsVisible == true);
            }

            if (request.MoedIds?.Count > 0)
            {
                logBag.Trace("Filter by MoedIds");
                query = query
                    .Where(x => request.MoedIds.Contains(x.MoedId));
            }

            if (request.QuestionnaireIds?.Count > 0)
            {
                logBag.Trace("Filter by QuestionnaireIds");
                query = query
                    .Where(x => request.QuestionnaireIds.Contains(x.QuestionnaireId));
            }

            if (request.ClassIds?.Count > 0 || request.GradeIds?.Count > 0)
            {
                logBag.Trace("Filter by ClassIds or GradeIds");
                var classIds = request.ClassIds ?? [];
                var gradeIds = request.GradeIds ?? [];

                var classGradeIds = request.ClassIds?.Count > 0 ? await _appDbContext.Classes
                    .Where(x => request.ClassIds.Contains(x.Id))
                    .Select(x => x.GradeId)
                    .ToListAsync() : [];

                var gradeClassIds = request.GradeIds?.Count > 0 ? await _appDbContext.Classes
                    .Where(x => request.GradeIds.Contains(x.GradeId))
                    .Select(x => x.Id)
                    .ToListAsync() : [];

                if (classGradeIds.Count != 0)
                {
                    gradeIds.AddRange(classGradeIds);
                    gradeIds = [.. gradeIds.Distinct()];
                }

                if (gradeClassIds.Count != 0)
                {
                    classIds.AddRange(gradeClassIds);
                    classIds = [.. classIds.Distinct()];
                }

                query = query.Where(x =>
                    x.ClassExams.Any(y => y.IsDeleted == false && classIds.Contains(y.ClassId)) ||
                    x.GradeExams.Any(y => y.IsDeleted == false && gradeIds.Contains(y.GradeId))
                  );
            }

            if (request.SubjectIds?.Count > 0)
            {
                logBag.Trace("Filter by SubjectIds");
                query = query
                    .Include(x => x.Questionnaire)
                    .Where(x => request.SubjectIds.Contains(x.Questionnaire.SubjectId));
            }

            var data = await query.OrderBy(x => x.StartTime).ToListAsync();
            logBag.AddOrUpdateParameter("DbRows", data.Count);

            var res = _mapper.Map<List<ExamDto>>(data);
            logBag.AddOrUpdateParameter("ResultRows", res.Count);

            logBag.Trace("Completed");
            return res;
        }

        public async Task<List<ExamDto>> GetAllAsync()
        {
            return await GetAllAsync(false);
        }

        public async Task<List<ExamDto>> GetAllAsync(bool filterCompletedExams)
        {
            var query = _appDbContext.Exams.Where(x => x.IsDeleted == false);
            if (filterCompletedExams)
            {
                query = query.Where(x => x.StartTime >= DateTimeOffset.Now);
            }

            if (IsFilterHiddenExams())
            {
                query = query
                  .Where(x => x.IsVisible == true);
            }

            var items = await query.OrderBy(x => x.StartTime)
              .ThenBy(x => x.QuestionnaireId)
              .ToListAsync();
            return _mapper.Map<List<ExamDto>>(items);
        }

        public async Task<ExamDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Exams
                .Include(x => x.ClassExams)
                .Include(x => x.GradeExams)
                .FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }
            return _mapper.Map<ExamDto>(item);
        }

        public async Task<ExamDto> InsertAsync(ExamPayloadDto model)
        {
            model.ClassIds ??= [];
            model.ImprovementClassIds ??= [];
            model.GradeIds ??= [];
            model.ImprovementGradeIds ??= [];

            await ValidationMethod(model);

            var exam = new Exam()
            {
                Duration = model.Duration,
                DurationWithExtra = model.DurationWithExtra,
                IsVisible = model.IsVisible,
                ExamTypeId = model.ExamTypeId,
                MoedId = model.MoedId,
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
                    IsImprovement = false,
                });
            }
            foreach (var classId in model.ImprovementClassIds)
            {
                await _appDbContext.ClassExams.AddAsync(new()
                {
                    ClassId = classId,
                    ExamId = exam.Id,
                    IsImprovement = true,
                });
            }
            foreach (var gradeId in model.GradeIds)
            {
                await _appDbContext.GradeExams.AddAsync(new()
                {
                    GradeId = gradeId,
                    ExamId = exam.Id,
                    IsImprovement = false,
                });
            }
            foreach (var gradeId in model.ImprovementGradeIds)
            {
                await _appDbContext.GradeExams.AddAsync(new()
                {
                    GradeId = gradeId,
                    ExamId = exam.Id,
                    IsImprovement = true,
                });
            }
            await _appDbContext.SaveChangesAsync();
            await _queueService.PublishMessageAsync(new Shared.QueueManager.Models.QueueMessage()
            {
                VirtualHost = BLConsts.QUEUE_VIRTUAL_HOST,
                QueueName = BLConsts.QUEUE_EXAM_EVENTS,
                Payload = JsonConvert.SerializeObject(exam.Id)
            });
            var res = _mapper.Map<ExamDto>(exam);
            res.ClassIds = model.ClassIds;
            res.ImprovementClassIds = model.ImprovementClassIds;
            res.GradeIds = model.GradeIds;
            res.ImprovementGradeIds = model.ImprovementGradeIds;
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
            model.ImprovementClassIds ??= [];
            model.GradeIds ??= [];
            model.ImprovementGradeIds ??= [];

            await ValidationMethod(model);

            item.Duration = model.Duration;
            item.IsVisible = model.IsVisible;
            item.DurationWithExtra = model.DurationWithExtra;
            item.ExamTypeId = model.ExamTypeId;
            item.MoedId = model.MoedId;
            item.QuestionnaireId = model.QuestionnaireId;
            item.StartTime = model.StartTime;
            await _appDbContext.SaveChangesAsync();

            {
                var dbClassExams = await _appDbContext.ClassExams.Where(x => x.IsDeleted == false && x.ExamId == item.Id).ToListAsync();
                var dbClassIds = dbClassExams.Select(x => x.ClassId).ToList();

                var dbClassIdsToDelete = dbClassIds.Where(x => !model.ClassIds.Contains(x) && !model.ImprovementClassIds.Contains(x)).ToList();
                if (dbClassIdsToDelete.Count > 0)
                {
                    var itemsToRemove = dbClassExams.Where(x => dbClassIdsToDelete.Contains(x.ClassId)).ToList();
                    itemsToRemove.ForEach(x => x.IsDeleted = true);
                }

                dbClassExams
                    .Where(x => model.ClassIds.Contains(x.ClassId))
                    .ToList()
                    .ForEach(x => x.IsImprovement = false);

                dbClassExams
                    .Where(x => model.ImprovementClassIds.Contains(x.ClassId))
                    .ToList()
                    .ForEach(x => x.IsImprovement = true);

                var classIdsToAdd = model.ClassIds.Where(x => !dbClassIds.Contains(x)).ToList();
                foreach (var classId in classIdsToAdd)
                {
                    await _appDbContext.ClassExams.AddAsync(new()
                    {
                        ClassId = classId,
                        ExamId = item.Id,
                        IsImprovement = false,
                    });
                }

                classIdsToAdd = model.ImprovementClassIds.Where(x => !dbClassIds.Contains(x)).ToList();
                foreach (var classId in classIdsToAdd)
                {
                    await _appDbContext.ClassExams.AddAsync(new()
                    {
                        ClassId = classId,
                        ExamId = item.Id,
                        IsImprovement = true,
                    });
                }
            }

            {
                var dbGradeExams = await _appDbContext.GradeExams.Where(x => x.IsDeleted == false && x.ExamId == item.Id).ToListAsync();
                var dbGradeIds = dbGradeExams.Select(x => x.GradeId).ToList();

                var dbGradeIdsToDelete = dbGradeIds.Where(x => !model.GradeIds.Contains(x) && !model.ImprovementGradeIds.Contains(x)).ToList();
                if (dbGradeIdsToDelete.Count > 0)
                {
                    var itemsToRemove = dbGradeExams.Where(x => dbGradeIdsToDelete.Contains(x.GradeId)).ToList();
                    itemsToRemove.ForEach(x => x.IsDeleted = true);
                }

                dbGradeExams
                    .Where(x => model.GradeIds.Contains(x.GradeId))
                    .ToList()
                    .ForEach(x => x.IsImprovement = false);

                dbGradeExams
                    .Where(x => model.ImprovementGradeIds.Contains(x.GradeId))
                    .ToList()
                    .ForEach(x => x.IsImprovement = true);

                var gradeIdsToAdd = model.GradeIds.Where(x => !dbGradeIds.Contains(x)).ToList();
                foreach (var gradeId in gradeIdsToAdd)
                {
                    await _appDbContext.GradeExams.AddAsync(new()
                    {
                        GradeId = gradeId,
                        ExamId = item.Id,
                        IsImprovement = false,
                    });
                }

                gradeIdsToAdd = model.ImprovementGradeIds.Where(x => !dbGradeIds.Contains(x)).ToList();
                foreach (var gradeId in gradeIdsToAdd)
                {
                    await _appDbContext.GradeExams.AddAsync(new()
                    {
                        GradeId = gradeId,
                        ExamId = item.Id,
                        IsImprovement = true,
                    });
                }
            }

            await _appDbContext.SaveChangesAsync();
            await _queueService.PublishMessageAsync(new Shared.QueueManager.Models.QueueMessage()
            {
                VirtualHost = BLConsts.QUEUE_VIRTUAL_HOST,
                QueueName = BLConsts.QUEUE_EXAM_EVENTS,
                Payload = JsonConvert.SerializeObject(item.Id)
            });
            var res = _mapper.Map<ExamDto>(item);
            res.ClassIds = model.ClassIds;
            res.ImprovementClassIds = model.ImprovementClassIds;
            res.GradeIds = model.GradeIds;
            res.ImprovementGradeIds = model.ImprovementGradeIds;
            return res;
        }

        public async Task<ExamDto?> SetAsVisibleAsync(Guid id)
        {
            var item = await _appDbContext.Exams.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return null;
            }
            item.IsVisible = true;
            await _appDbContext.SaveChangesAsync();
            await _queueService.PublishMessageAsync(new Shared.QueueManager.Models.QueueMessage()
            {
                VirtualHost = BLConsts.QUEUE_VIRTUAL_HOST,
                QueueName = BLConsts.QUEUE_EXAM_EVENTS,
                Payload = JsonConvert.SerializeObject(item.Id)
            });
            return await GetAsync(id);
        }

        private async Task ValidationMethod(ExamPayloadDto model)
        {
            model.ClassIds ??= [];
            model.ImprovementClassIds ??= [];
            model.GradeIds ??= [];
            model.ImprovementGradeIds ??= [];

            var examTypeIsExists = await _appDbContext.ExamTypes.AnyAsync(x => x.IsDeleted == false && x.Id == model.ExamTypeId);
            if (!examTypeIsExists)
            {
                throw new BadRequestException("Exam type Id not found");
            }

            var moedIsExists = await _appDbContext.Moeds.AnyAsync(x => x.IsDeleted == false && x.Id == model.MoedId);
            if (!moedIsExists)
            {
                throw new BadRequestException("Moed Id not found");
            }

            var questionnaireIsExists = await _appDbContext.Questionnaires.AnyAsync(x => x.IsDeleted == false && x.Id == model.QuestionnaireId);
            if (!questionnaireIsExists)
            {
                throw new BadRequestException("Questionnaire Id not found");
            }

            var dbClassIds = await _appDbContext.Classes.Where(x => x.IsDeleted == false).Select(x => x.Id).ToListAsync();
            var invalidClassIds = model.ClassIds.Union(model.ImprovementClassIds).Where(x => !dbClassIds.Contains(x)).ToList();
            if (invalidClassIds.Count != 0)
            {
                throw new BadRequestException($"Invalid class Id's [{string.Join(", ", invalidClassIds)}]");
            }

            var dbGradeIds = await _appDbContext.Grades.Where(x => x.IsDeleted == false).Select(x => x.Id).ToListAsync();
            var invalidGradeIds = model.GradeIds.Union(model.ImprovementGradeIds).Where(x => !dbGradeIds.Contains(x)).ToList();
            if (invalidGradeIds.Count != 0)
            {
                throw new BadRequestException($"Invalid grade Id's [{string.Join(", ", invalidGradeIds)}]");
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
            await _queueService.PublishMessageAsync(new Shared.QueueManager.Models.QueueMessage()
            {
                VirtualHost = BLConsts.QUEUE_VIRTUAL_HOST,
                QueueName = BLConsts.QUEUE_EXAM_EVENTS,
                Payload = JsonConvert.SerializeObject(item.Id)
            });
            return true;
        }
    }
}
