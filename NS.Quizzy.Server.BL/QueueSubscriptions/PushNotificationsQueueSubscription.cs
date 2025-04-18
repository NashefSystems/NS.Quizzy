using Microsoft.EntityFrameworkCore;
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
            var notificationId = JsonConvert.DeserializeObject<Guid?>(message.Payload);
            if (notificationId == null || notificationId == Guid.Empty)
            {
                logBag.LogLevel = NSLogLevel.Warn;
                return res.SetOk("Notification id is null or empty");
            }

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var fcmService = scope.ServiceProvider.GetRequiredService<IFcmService>();

            var notification = await dbContext.Notifications
                .Include(x => x.UserNotifications)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == notificationId.Value && x.IsDeleted == false);

            if (notification == null)
            {
                logBag.LogLevel = NSLogLevel.Warn;
                return res.SetOk("Notification is null");
            }

            var successPn = new List<Guid>();
            var errorPn = new List<Guid>();
            var users = notification.UserNotifications
                .Where(x => x.User.IsDeleted == false && !string.IsNullOrWhiteSpace(x.User.NotificationToken))
                .Select(x => x.User)
                .ToList();

            logBag.Trace($"Found {users.Count} users with notification token");
            for (int i = 0; i < users.Count; i++)
            {
                await setMessageProgressPercentage(Math.Truncate(100.0 * i / users.Count));
                var request = new PushNotificationRequest()
                {
                    DeviceToken = users[i].NotificationToken,
                    Title = notification.Title,
                    Body = notification.Body,
                    Data = notification.Data
                };
                var isSuccess = await fcmService.SendPushNotificationAsync(request);
                if (isSuccess)
                {
                    successPn.Add(users[i].Id);
                }
                else
                {
                    errorPn.Add(users[i].Id);
                }
            }
            await setMessageProgressPercentage(100);
            logBag.AddOrUpdateParameter("SuccessPushNotifications", successPn);
            logBag.AddOrUpdateParameter("ErrorPushNotifications", errorPn);
            if (errorPn.Count > 0)
            {
                logBag.LogLevel = NSLogLevel.Warn;
                logBag.Trace($"Error sending push notifications to users: {string.Join(", ", errorPn)}");
            }

            if (errorPn.Count == 0)
            {
                return res.SetOk("Push notifications sent successfully");
            }
            else if (successPn.Count == 0)
            {
                logBag.LogLevel = NSLogLevel.Error;
                return res.SetError("Failed to send push notifications to all users");
            }

            logBag.LogLevel = NSLogLevel.Warn;
            return res.SetOk("Push notifications sent partially");
        }
    }
}
