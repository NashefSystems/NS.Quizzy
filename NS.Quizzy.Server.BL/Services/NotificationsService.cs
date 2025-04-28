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
using NS.Shared.QueueManager.Models;
using Microsoft.Extensions.Configuration;
using static NS.Quizzy.Server.Common.Enums;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Utils;

namespace NS.Quizzy.Server.BL.Services
{
    internal class NotificationsService : INotificationsService
    {
        private readonly INSQueueService _queueService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly INSLogger _logger;
        private readonly int _notificationsGetLimitValue;

        public NotificationsService(IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext, IMapper mapper, INSQueueService queueService, INSLogger logger, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _queueService = queueService;
            _logger = logger;

            {
                int value = 50; // default
                var cacheKey = AppSettingKeys.NotificationsGetLimitValue.GetDBStringValue();
                if (int.TryParse(configuration.GetValue<string>(cacheKey), out int val) && val > 0)
                {
                    value = val;
                }
                _notificationsGetLimitValue = value;
            }
        }

        public async Task<int> GetNumberOfMyNewNotificationsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var count = await _appDbContext.UserNotifications
                .Include(x => x.Notification)
                .Where(x =>
                    x.IsDeleted == false &&
                    x.Notification.IsDeleted == false &&
                    x.UserId == userId &&
                    x.SeenAt.HasValue == false
                )
                .Select(x => x.Notification)
                .CountAsync();

            return count;
        }

        public async Task<List<MyNotificationItem>> GetMyNotificationsAsync(int? limit)
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var res = await _appDbContext.UserNotifications
                .Include(x => x.Notification)
                .ThenInclude(x => x.CreatedBy)
                .Where(x =>
                    x.IsDeleted == false &&
                    x.Notification.IsDeleted == false &&
                    x.UserId == userId
                )
                .OrderByDescending(x => x.Notification.CreatedTime)
                .Select(x => new MyNotificationItem()
                {
                    Id = x.Notification.Id,
                    Title = x.Notification.Title,
                    Body = x.Notification.Body,
                    CreatedTime = x.Notification.CreatedTime,
                    Data = x.Notification.Data,
                    Read = x.SeenAt.HasValue,
                    Author = x.Notification.CreatedBy.FullName,
                })
                .Take(limit ?? int.MaxValue)
                .ToListAsync();
            return res;
        }

        public async Task<MyNotificationItem> MarkAsReadAsync(Guid notificationId)
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var item = await _appDbContext.UserNotifications
                .Include(x => x.Notification)
                .ThenInclude(x => x.CreatedBy)
                .FirstOrDefaultAsync(x =>
                    x.IsDeleted == false &&
                    x.Notification.IsDeleted == false &&
                    x.Notification.Id == notificationId &&
                    x.UserId == userId
                ) ?? throw new BadRequestException("Notification not found");

            item.SeenAt = DateTimeOffset.Now;
            await _appDbContext.SaveChangesAsync();

            return new MyNotificationItem()
            {
                Id = item.Notification.Id,
                Title = item.Notification.Title,
                Body = item.Notification.Body,
                CreatedTime = item.Notification.CreatedTime,
                Data = item.Notification.Data,
                Read = item.SeenAt.HasValue,
                Author = item.Notification.CreatedBy.FullName,
            };
        }

        public async Task<List<NotificationDto>> GetAllAsync()
        {
            var items = await _appDbContext.Notifications
                .Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedTime)
                .Take(_notificationsGetLimitValue)
                .ToListAsync();

            var notificationIds = items.Select(x => x.Id).ToList();

            var totalUsersDic = await _appDbContext.UserNotifications
                .AsNoTracking()
                .Where(x => x.IsDeleted == false)
                .GroupBy(g => g.NotificationId)
                .ToDictionaryAsync(k => k.Key, v => v.Count());

            var totalReadDic = await _appDbContext.UserNotifications
                .AsNoTracking()
                .Where(x => x.IsDeleted == false && x.SeenAt.HasValue)
                .GroupBy(g => g.NotificationId)
                .ToDictionaryAsync(k => k.Key, v => v.Count());

            var numberOfPushNotificationsReceivedDic = await _appDbContext.UserNotifications
                .AsNoTracking()
                .Where(x => x.IsDeleted == false && x.PushNotificationsSendingTime.HasValue)
                .GroupBy(g => g.NotificationId)
                .ToDictionaryAsync(k => k.Key, v => v.Count());

            var res = items.Select(x =>
            {
                int? totalUsers = totalUsersDic.ContainsKey(x.Id) ? totalUsersDic[x.Id] : null;
                int? totalRead = totalReadDic.ContainsKey(x.Id) ? totalReadDic[x.Id] : null;
                int? numberOfPushNotificationsReceived = numberOfPushNotificationsReceivedDic.ContainsKey(x.Id) ? numberOfPushNotificationsReceivedDic[x.Id] : null;
                var res = _mapper.Map<NotificationDto>(x);

                res.TotalUsers = totalUsers;
                res.TotalRead = totalRead;
                res.ReadPercentage = NumberUtils.GetPercentage(totalRead, totalUsers);
                res.NumberOfPushNotificationsReceived = numberOfPushNotificationsReceived;
                res.PushNotificationReceivedPercentage = NumberUtils.GetPercentage(numberOfPushNotificationsReceived, totalUsers);
                return res;
            }).ToList();

            return res;
        }

        public async Task<NotificationDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Notifications
                 .Include(x => x.UserNotifications)
                 .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (item != null)
            {
                item.UserNotifications = item.UserNotifications.Where(x => x.IsDeleted == false).ToList();
            }
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
                TargetIds = model.TargetIds,
                CreatedById = myUserId.Value,
                UserNotifications = [],
            };
            await _appDbContext.Notifications.AddAsync(item);

            List<Guid> userIds = await GetTargetUserIds(item.Target, item.TargetIds);
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

            #region AddItemsToQueue
            List<PublishMessageResponse> taskResults = [];
            var batchSize = 15;
            var skip = 0;
            var tasks = item.UserNotifications.Skip(skip).Take(batchSize)
                .Select(x => _queueService.PublishMessageAsync(new Shared.QueueManager.Models.NSQueueMessage()
                {
                    QueueName = BLConsts.QUEUE_PUSH_NOTIFICATIONS,
                    Payload = JsonConvert.SerializeObject(x.Id)
                }))
                .ToList();
            while (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
                taskResults.AddRange(tasks.Select(x => x.Result));

                skip += batchSize;
                tasks = item.UserNotifications.Skip(skip).Take(batchSize)
                    .Select(x => _queueService.PublishMessageAsync(new Shared.QueueManager.Models.NSQueueMessage()
                    {
                        QueueName = BLConsts.QUEUE_PUSH_NOTIFICATIONS,
                        Payload = JsonConvert.SerializeObject(x.Id)
                    }))
                    .ToList();
            }
            var total = taskResults.Count();
            var numberOfFailedRequests = taskResults.Where(x => !x.IsSuccessful).Count();
            logBag.AddOrUpdateParameter("PublishQueueMessageResult", new
            {
                Total = total,
                NumberOfSuccessfulRequests = total - numberOfFailedRequests,
                NumberOfFailedRequests = numberOfFailedRequests,
            });
            logBag.AddOrUpdateParameter("PublishQueueMessageErrorResults", taskResults.Where(x => !x.IsSuccessful).ToList());

            if (numberOfFailedRequests > 0)
            {
                logBag.LogLevel = NSLogLevel.Warn;
            }
            #endregion

            return _mapper.Map<NotificationDto>(item);
        }

        private async Task<List<Guid>> GetTargetUserIds(NotificationTarget target, List<Guid>? tIds)
        {
            List<Guid> targetIds = tIds ?? [];
            var userIds = new List<Guid>();
            targetIds ??= [];
            switch (target)
            {
                case NotificationTarget.SpecificUsers:
                    {
                        userIds.AddRange(targetIds);
                        break;
                    }
                case NotificationTarget.Classes:
                    {
                        var ids = await _appDbContext.Users
                            .Where(x => x.IsDeleted == false && x.ClassId.HasValue && targetIds.Contains(x.ClassId.Value))
                            .Select(x => x.Id)
                            .ToListAsync();
                        if (ids.Count > 0)
                        {
                            userIds.AddRange(ids);
                        }
                        break;
                    }
                case NotificationTarget.Grades:
                    {
                        var classIds = await _appDbContext.Classes
                            .Where(x => x.IsDeleted == false && targetIds.Contains(x.GradeId))
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
                        break;
                    }
                case NotificationTarget.Students:
                case NotificationTarget.Teachers:
                case NotificationTarget.TeachersAndStudents:
                    {
                        List<Roles> roles = [];
                        if (target == NotificationTarget.Students || target == NotificationTarget.TeachersAndStudents)
                        {
                            roles.Add(Roles.Student);
                        }
                        if (target == NotificationTarget.Teachers || target == NotificationTarget.TeachersAndStudents)
                        {
                            roles.Add(Roles.Teacher);
                        }

                        var ids = await _appDbContext.Users
                            .Where(x => x.IsDeleted == false && roles.Contains(x.Role))
                            .Select(x => x.Id)
                            .ToListAsync();

                        if (ids.Count > 0)
                        {
                            userIds.AddRange(ids);
                        }
                        break;
                    }
                case NotificationTarget.Admins:
                    {
                        List<Roles> roles = [Roles.Admin, Roles.Developer, Roles.SuperAdmin];

                        var ids = await _appDbContext.Users
                            .Where(x => x.IsDeleted == false && roles.Contains(x.Role))
                            .Select(x => x.Id)
                            .ToListAsync();

                        if (ids.Count > 0)
                        {
                            userIds.AddRange(ids);
                        }
                        break;
                    }
                case NotificationTarget.AllUsers:
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
            var item = await _appDbContext.Notifications.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
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
