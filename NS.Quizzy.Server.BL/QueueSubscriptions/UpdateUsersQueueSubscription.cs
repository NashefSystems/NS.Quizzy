using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Extensions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.Logging;
using NS.Shared.QueueManager.Models;
using NS.Shared.QueueManager.Services;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.QueueSubscriptions
{
    internal class UpdateUsersQueueSubscription : QueueSubscriptionBase
    {
        public override int GetMaximumAttempts() => 2;
        public override string GetVirtualHost() => BLConsts.QUEUE_VIRTUAL_HOST;
        public override string GetQueueName() => BLConsts.QUEUE_UPDATE_USERS;

        public override async Task<QueueSubscriptionAcceptMethodResult> ProcessMessageAsync(Guid messageId, QueueMessage message, INSLogBag logBag, IServiceScope scope, CancellationToken cancellationToken)
        {
            logBag.LogLevel = NSLogLevel.Info;
            var res = new QueueSubscriptionAcceptMethodResult();
            string messageStatusCacheKey = messageId.GetQueueMessageStatusCacheKey();
            MessageStatusInfo? messageStatusInfo = null;
            INSCacheProvider? _cacheProvider = null;
            INSLogger? _logger = null;
            try
            {
                logBag.Trace("Starting ProcessMessageAsync");
                logBag.AddOrUpdateParameter(nameof(messageStatusCacheKey), messageStatusCacheKey);

                _cacheProvider = scope.ServiceProvider.GetRequiredService<INSCacheProvider>();
                _logger = scope.ServiceProvider.GetRequiredService<INSLogger>();
                messageStatusInfo = await _cacheProvider.GetAsync<MessageStatusInfo>(messageStatusCacheKey);
                messageStatusInfo ??= new MessageStatusInfo();
                messageStatusInfo.AddContextId(logBag?.Logger?.GetContextId());
                messageStatusInfo.DownloadCounter += 1;
                await SetMessageProgressPercentageAsync(_cacheProvider, messageStatusCacheKey, messageStatusInfo, 0);
                var items = JsonConvert.DeserializeObject<List<CsvFileItem>>(message.Payload);
                if (items == null || items.Count == 0)
                {
                    logBag.LogLevel = NSLogLevel.Warn;
                    return res.SetOk("Item is null or empty");
                }

                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();
                var configKey = AppSettingKeys.IdNumberEmailDomain.GetDBStringValue();
                var idNumberEmailDomain = configuration.GetValue<string>(configKey);

                var userDic = await dbContext.Users
                    .Where(x => x.IsDeleted == false && (x.Role == DALEnums.Roles.Student || x.Role == DALEnums.Roles.Teacher))
                    .ToDictionaryAsync(k => k.Email.ToLower(), v => v);

                var classDic = await dbContext.Classes
                    .Include(x => x.Grade)
                    .Where(x => x.IsDeleted == false)
                    .ToDictionaryAsync(k => k.GetFullCode().ToString(), v => v.Id);

                foreach (var item in userDic.Values)
                {
                    item.IsDeleted = true;
                }

                await SetMessageProgressPercentageAsync(_cacheProvider, messageStatusCacheKey, messageStatusInfo, 0);
                for (int i = 0; i < items.Count; i++)
                {
                    await SetMessageProgressPercentageAsync(_cacheProvider, messageStatusCacheKey, messageStatusInfo, 100.0 * i / items.Count);
                    var email = $"{items[i].IdNumber}@{idNumberEmailDomain}";
                    var role = items[i].Role.ToUserRole();
                    Guid? classId = null;
                    if (classDic.TryGetValue(items[i].Class, out var _classId))
                    {
                        classId = _classId;
                    }

                    if (!userDic.TryGetValue(email.ToLower(), out var user))
                    {
                        user = new User();
                        await dbContext.Users.AddAsync(user);
                        userDic.Add(email.ToLower(), user);
                    }

                    user.Email = email;
                    user.Password = email.ToLower();
                    user.IdNumber = items[i].IdNumber;
                    user.FullName = items[i].FullName;
                    user.Role = role;
                    user.ClassId = role == DALEnums.Roles.Teacher ? null : classId;
                    user.IsDeleted = false;
                }
                await dbContext.SaveChangesAsync();
                await usersService.ClearCacheAsync();
                await SuccessHandler(_cacheProvider, messageStatusCacheKey, messageStatusInfo, logBag);
                return res.SetOk();
            }
            catch (Exception ex)
            {
                logBag.Exception = ex;
                await ErrorHandlerAsync(_cacheProvider, _logger, messageStatusCacheKey, messageStatusInfo, ex.Message, logBag);
                return res.SetError(ex.Message);
            }
        }

        private async Task SuccessHandler(INSCacheProvider cacheProvider, string cacheKey, MessageStatusInfo? statusInfo, INSLogBag logBag)
        {
            statusInfo ??= new MessageStatusInfo();
            statusInfo.AddContextId(logBag?.Logger?.GetContextId());
            statusInfo.Error = null;
            statusInfo.IsCompleted = true;
            statusInfo.IsSuccess = true;
            statusInfo.ProgressPercentage = 100;
            await UpdateMessageStatusInfoAsync(cacheProvider, cacheKey, statusInfo);
        }

        private async Task ErrorHandlerAsync(INSCacheProvider? cacheProvider, INSLogger? logger, string cacheKey, MessageStatusInfo? statusInfo, string error, INSLogBag logBag)
        {
            try
            {
                if (cacheProvider == null)
                {
                    return;
                }
                statusInfo ??= new MessageStatusInfo();
                statusInfo.AddContextId(logBag?.Logger?.GetContextId());
                statusInfo.Error = error;
                statusInfo.IsCompleted = true;
                statusInfo.IsSuccess = false;
                await UpdateMessageStatusInfoAsync(cacheProvider, cacheKey, statusInfo);
            }
            catch (Exception ex)
            {
                logger?.Fatal(ex, nameof(ErrorHandlerAsync), new { cacheKey });
            }
        }

        private async Task SetMessageProgressPercentageAsync(INSCacheProvider cacheProvider, string cacheKey, MessageStatusInfo statusInfo, double progressPercentage)
        {
            if (progressPercentage < 0) progressPercentage = 0;
            if (progressPercentage > 100) progressPercentage = 100;
            statusInfo.ProgressPercentage = Math.Truncate(progressPercentage);
            await UpdateMessageStatusInfoAsync(cacheProvider, cacheKey, statusInfo);
        }

        private async Task UpdateMessageStatusInfoAsync(INSCacheProvider cacheProvider, string cacheKey, MessageStatusInfo statusInfo)
        {
            await cacheProvider.SetOrUpdateAsync(cacheKey, statusInfo, TimeSpan.FromDays(BLConsts.MESSAGE_STATUS_CACHE_TTL_IN_DAYS));
        }
    }
}
