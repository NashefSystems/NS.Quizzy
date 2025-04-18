using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.BL.DTOs;
using Microsoft.AspNetCore.Http;
using NS.Quizzy.Server.BL.Extensions;
using static NS.Quizzy.Server.DAL.DALEnums;
using Newtonsoft.Json;
using NS.Shared.QueueManager.Interfaces;
using NS.Shared.Logging;

namespace NS.Quizzy.Server.BL.Services
{
    internal class NotificationsService : INotificationsService
    {
        private readonly INSQueueService _queueService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly INSLogger _logger;

        public NotificationsService(IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext, IMapper mapper, INSQueueService queueService, INSLogger logger/*, IConfiguration configuration*/)
        {
            _httpContextAccessor = httpContextAccessor;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _queueService = queueService;
            _logger = logger;
        }

        public async Task<List<NotificationDto>> GetMyNotificationsAsync(bool isArchive)
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var notificationIds = await _appDbContext.UserNotifications
                .Where(x => x.UserId == userId && x.SeenAt.HasValue == isArchive)
                .Select(x => x.NotificationId)
                .ToListAsync();

            var items = await _appDbContext.Notifications
                .Where(x => x.IsDeleted == false && notificationIds.Contains(x.Id))
                .OrderByDescending(x => x.CreatedTime)
                .ToListAsync();

            var res = _mapper.Map<List<NotificationDto>>(items);
            return res;
        }

        public async Task<List<NotificationDto>> GetAllAsync()
        {
            var items = await _appDbContext.Notifications
                .Include(x => x.UserNotifications)
                .Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedTime)
                .ToListAsync();
            var res = _mapper.Map<List<NotificationDto>>(items);
            return res;
        }

        public async Task<NotificationDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Notifications
                 .Include(x => x.UserNotifications)
                 .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            return _mapper.Map<NotificationDto>(item);
        }

        public async Task<NotificationDto> InsertAsync(NotificationPayloadDto model)
        {
            using var logBag = _logger.CreateLogBag("InsertAsync");
            var myUserId = _httpContextAccessor.HttpContext.GetUserId();
            if (!myUserId.HasValue)
            {
                logBag.Trace("User not found");
                logBag.LogLevel = NSLogLevel.Error;
                throw new UnauthorizedAccessException("User not found");
            }

            var item = new DAL.Entities.Notification()
            {
                Title = model.Title,
                Body = model.Body,
                Data = model.Data,
                Target = model.Target,
                P1 = model.P1,
                P2 = model.P2,
                P3 = model.P3,
                CreatedBy = myUserId.Value,
                UserNotifications = [],
            };
            await _appDbContext.Notifications.AddAsync(item);

            List<Guid> userIds = await GetTargetUserIds(item.Target, item.P1);
            logBag.AddOrUpdateParameter("TargetUserIds", userIds);
            if (userIds.Count != 0)
            {
                var users = await _appDbContext.Users
                    .Where(x => x.IsDeleted == false && userIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var user in users)
                {
                    item.UserNotifications.Add(new DAL.Entities.UserNotification()
                    {
                        UserId = user.Id,
                        User = user,
                    });
                }
            }
            await _appDbContext.SaveChangesAsync();
            logBag.AddOrUpdateParameter("NotificationId", item.Id);


            var publishMessageResult = await _queueService.PublishMessageAsync(new Shared.QueueManager.Models.NSQueueMessage()
            {
                QueueName = BLConsts.QUEUE_PUSH_NOTIFICATIONS,
                Payload = JsonConvert.SerializeObject(item.Id)
            });

            logBag.AddOrUpdateParameter("PublishQueueMessageResult", publishMessageResult);

            if (!publishMessageResult.IsSuccessful)
            {
                logBag.LogLevel = NSLogLevel.Warn;
                logBag.Trace($"Unable to push queue message: '{publishMessageResult.Error}'");
            }

            return _mapper.Map<NotificationDto>(item);
        }

        private async Task<List<Guid>> GetTargetUserIds(NotificationTarget target, string? p1)
        {
            var userIds = new List<Guid>();
            switch (target)
            {
                case NotificationTarget.User:
                    {
                        if (Guid.TryParse(p1, out var userId))
                        {
                            userIds.Add(userId);
                        }
                        break;
                    }
                case NotificationTarget.Class:
                    {
                        if (Guid.TryParse(p1, out var classId))
                        {
                            var ids = await _appDbContext.Users
                                .Where(x => x.IsDeleted == false && x.ClassId == classId)
                                .Select(x => x.Id)
                                .ToListAsync();
                            if (ids.Count > 0)
                            {
                                userIds.AddRange(ids);
                            }
                        }
                        break;
                    }
                case NotificationTarget.Grade:
                    {
                        if (Guid.TryParse(p1, out var gradeId))
                        {
                            var classIds = await _appDbContext.Classes
                                .Where(x => x.IsDeleted == false && x.GradeId == gradeId)
                                .Select(x => x.Id)
                                .ToListAsync();

                            var ids = await _appDbContext.Users
                                .Where(x => x.IsDeleted == false && x.ClassId.HasValue && classIds.Contains(x.ClassId.Value))
                                .Select(x => x.Id)
                                .ToListAsync();

                            if (ids.Count > 0)
                            {
                                userIds.AddRange(ids);
                            }
                        }
                        break;
                    }
                case NotificationTarget.Role:
                    {
                        if (!Enum.TryParse(p1, out Roles userRole))
                        {
                            var ids = await _appDbContext.Users
                                .Where(x => x.IsDeleted == false && x.Role == userRole)
                                .Select(x => x.Id)
                                .ToListAsync();

                            if (ids.Count > 0)
                            {
                                userIds.AddRange(ids);
                            }
                        }
                        break;
                    }
                case NotificationTarget.All:
                    {
                        var ids = await _appDbContext.Users
                                .Where(x => x.IsDeleted == false)
                                .Select(x => x.Id)
                                .ToListAsync();

                        if (ids.Count > 0)
                        {
                            userIds.AddRange(ids);
                        }
                        break;
                    }
            }

            return userIds;
        }

        public Task<NotificationDto?> UpdateAsync(Guid id, NotificationPayloadDto model)
        {
            throw new NotSupportedException();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            throw new NotSupportedException();
        }
    }
}
