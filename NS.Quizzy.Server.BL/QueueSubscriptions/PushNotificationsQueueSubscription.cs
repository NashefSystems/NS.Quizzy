﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.DAL;
using NS.Shared.Logging;
using NS.Shared.QueueManager.Models;
using NS.Shared.QueueManager.Services;

namespace NS.Quizzy.Server.BL.QueueSubscriptions
{
    internal class PushNotificationsQueueSubscription : QueueSubscriptionBase
    {
        public override int GetMaximumAttempts() => 2;
        public override string GetQueueName() => BLConsts.QUEUE_PUSH_NOTIFICATIONS;

        public override async Task<QueueSubscriptionAcceptMethodResult> ProcessMessageAsync(NSQueueMessage message, Func<double, Task> setMessageProgressPercentage, INSLogBag logBag, IServiceScope scope, CancellationToken cancellationToken)
        {
            var res = new QueueSubscriptionAcceptMethodResult();
            logBag.Trace("Starting ProcessMessageAsync");
            await setMessageProgressPercentage(0);
            var userNotificationId = JsonConvert.DeserializeObject<Guid?>(message.Payload);
            if (userNotificationId == null || userNotificationId == Guid.Empty)
            {
                logBag.LogLevel = NSLogLevel.Warn;
                return res.SetOk("User notification id is null or empty");
            }

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var fcmService = scope.ServiceProvider.GetRequiredService<IFcmService>();
            await setMessageProgressPercentage(30);
            var userNotification = await dbContext.UserNotifications
                .Include(x => x.User)
                .Include(x => x.Notification)
                .FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == userNotificationId.Value);

            if (userNotification == null)
            {
                logBag.LogLevel = NSLogLevel.Warn;
                return res.SetOk("User notification is null");
            }

            if (userNotification.Notification.IsDeleted)
            {
                logBag.LogLevel = NSLogLevel.Warn;
                return res.SetOk("Notification is deleted");
            }

            if (userNotification.User.IsDeleted)
            {
                logBag.LogLevel = NSLogLevel.Warn;
                return res.SetOk("User is deleted");
            }

            if (string.IsNullOrWhiteSpace(userNotification.User.NotificationToken))
            {
                return res.SetOk("User notification token is null or empty");
            }

            await setMessageProgressPercentage(60);
            var request = new PushNotificationRequest()
            {
                DeviceToken = userNotification.User.NotificationToken,
                Title = userNotification.Notification.Title,
                Body = userNotification.Notification.Body,
                Data = userNotification.Notification.Data ?? []
            };
            request.Data["userNotificationId"] = userNotification.Id.ToString();
            request.Data["notificationId"] = userNotification.Notification.Id.ToString();
            var isSuccess = await fcmService.SendPushNotificationAsync(request);
            await setMessageProgressPercentage(100);
            if (isSuccess)
            {
                userNotification.PushNotificationsSendingTime = DateTimeOffset.Now;
                await dbContext.SaveChangesAsync(cancellationToken);
                return res.SetOk("Push notification sent successfully");
            }
            else
            {
                logBag.LogLevel = NSLogLevel.Error;
                return res.SetError("Failed to send push notifications to all users");
            }
        }
    }
}
