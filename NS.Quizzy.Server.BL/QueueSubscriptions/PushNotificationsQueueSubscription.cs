using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Extensions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.Logging;
using NS.Shared.QueueManager.Models;
using NS.Shared.QueueManager.Services;

namespace NS.Quizzy.Server.BL.QueueSubscriptions
{
    internal class PushNotificationsQueueSubscription : QueueSubscriptionBase
    {
        public override int GetMaximumAttempts() => 2;
        public override string GetVirtualHost() => BLConsts.QUEUE_VIRTUAL_HOST;
        public override string GetQueueName() => BLConsts.QUEUE_PUSH_NOTIFICATIONS;

        public override async Task<QueueSubscriptionAcceptMethodResult> ProcessMessageAsync(Guid messageId, QueueMessage message, INSLogBag logBag, IServiceScope scope, CancellationToken cancellationToken)
        {
            logBag.LogLevel = NSLogLevel.Info;
            var res = new QueueSubscriptionAcceptMethodResult();

            string messageStatusCacheKey = messageId.GetQueueMessageStatusCacheKey();
            MessageStatusInfo? messageStatusInfo = null;
            INSCacheProvider? _cacheProvider = null;
            try
            {
                logBag.Trace("Starting ProcessMessageAsync");
                logBag.AddOrUpdateParameter(nameof(messageId), messageId);
                logBag.AddOrUpdateParameter(nameof(messageStatusCacheKey), messageStatusCacheKey);

                _cacheProvider = scope.ServiceProvider.GetRequiredService<INSCacheProvider>();
                messageStatusInfo = await _cacheProvider.GetAsync<MessageStatusInfo>(messageStatusCacheKey);
                messageStatusInfo ??= new MessageStatusInfo();
                messageStatusInfo.AddContextId(logBag.Logger?.GetContextId());
                messageStatusInfo.DownloadCounter += 1;

                await SetMessageProgressPercentageAsync(_cacheProvider, messageStatusCacheKey, messageStatusInfo, 0);

                #region logic
                var userNotificationId = JsonConvert.DeserializeObject<Guid?>(message.Payload);
                if (!userNotificationId.HasValue || userNotificationId == Guid.Empty)
                {
                    throw new NullReferenceException(nameof(userNotificationId));
                }
                logBag.AddOrUpdateParameter("UserNotificationId", userNotificationId);

                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var fcmService = scope.ServiceProvider.GetRequiredService<IFcmService>();
                var userNotification = await dbContext.UserNotifications
                    .Include(x => x.User)
                    .Include(x => x.Notification)
                    .FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == userNotificationId.Value);

                if (userNotification == null)
                {
                    throw new NullReferenceException(nameof(userNotification));
                }

                logBag.AddOrUpdateParameter("NotificationId", userNotification.NotificationId);
                logBag.AddOrUpdateParameter("UserId", userNotification.UserId);

                if (userNotification.Notification.IsDeleted)
                {
                    logBag.LogLevel = NSLogLevel.Warn;
                    logBag.Trace("Notification is deleted");
                    return res.SetOk("Notification is deleted");
                }

                if (userNotification.User.IsDeleted)
                {
                    logBag.LogLevel = NSLogLevel.Warn;
                    logBag.Trace("User is deleted");
                    return res.SetOk("User is deleted");
                }

                if (string.IsNullOrWhiteSpace(userNotification.User.NotificationToken))
                {
                    logBag.Trace("User notification token is null or empty");
                    return res.SetOk("User notification token is null or empty");
                }

                var request = new PushNotificationRequest()
                {
                    DeviceToken = userNotification.User.NotificationToken,
                    Title = userNotification.Notification.Title,
                    Body = userNotification.Notification.Body,
                    Data = userNotification.Notification.Data ?? []
                };
                request.Data["userNotificationId"] = userNotification.Id.ToString();
                request.Data["notificationId"] = userNotification.Notification.Id.ToString();
                var isSuccess = await fcmService.SendPushNotificationAsync(request, logBag);
                if (!isSuccess)
                {
                    logBag.LogLevel = NSLogLevel.Error;
                    return res.SetError("Failed to send push notifications to all users");
                }
                #endregion
                userNotification.PushNotificationsSendingTime = DateTimeOffset.Now;
                await dbContext.SaveChangesAsync(cancellationToken);
                await SuccessHandler(_cacheProvider, messageStatusCacheKey, messageStatusInfo, logBag);
                return res.SetOk("Push notification sent successfully");
            }
            catch (Exception ex)
            {
                logBag.Exception = ex;
                await ErrorHandlerAsync(_cacheProvider, logBag.Logger, messageStatusCacheKey, messageStatusInfo, ex.Message, logBag);
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
