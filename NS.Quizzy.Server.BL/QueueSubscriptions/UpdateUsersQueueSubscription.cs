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
using NS.Shared.Logging;
using NS.Shared.QueueManager.Models;
using NS.Shared.QueueManager.Services;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.QueueSubscriptions
{
    internal class UpdateUsersQueueSubscription : QueueSubscriptionBase
    {
        public override int GetMaximumAttempts() => 2;
        public override string GetQueueName() => BLConsts.QUEUE_UPDATE_USERS;

        public override async Task<QueueSubscriptionAcceptMethodResult> ProcessMessageAsync(NSQueueMessage message, INSLogBag logBag, IServiceScope scope, CancellationToken cancellationToken)
        {
            var res = new QueueSubscriptionAcceptMethodResult();
            logBag.Trace("Starting ProcessMessageAsync");
            var items = JsonConvert.DeserializeObject<List<CsvFileItem>>(message.Payload);
            if (items == null || items.Count == 0)
            {
                logBag.LogLevel = NSLogLevel.Warn;
                return res.SetOk("Item is null or empty");
            }

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();
            var cacheKey = AppSettingKeys.IdNumberEmailDomain.GetDBStringValue();
            var idNumberEmailDomain = configuration.GetValue<string>(cacheKey);

            var userDic = await dbContext.Users
                .Where(x => x.Role == DALEnums.Roles.Student || x.Role == DALEnums.Roles.Teacher)
                .ToDictionaryAsync(k => k.Email.ToLower(), v => v);

            var classDic = await dbContext.Classes
                .Include(x => x.Grade)
                .Where(x => x.IsDeleted == false)
                .ToDictionaryAsync(k => k.GetFullCode().ToString(), v => v.Id);

            foreach (var item in userDic.Values)
            {
                item.IsDeleted = true;
            }

            foreach (var item in items)
            {
                var email = $"{item.IdNumber}@{idNumberEmailDomain}";
                var role = item.Role.ToUserRole();
                //item.FullName;
                classDic.TryGetValue(item.Class, out var classId);

                if (!userDic.TryGetValue(email.ToLower(), out var user))
                {
                    user = new User();
                    await dbContext.Users.AddAsync(user);
                    userDic.Add(email.ToLower(), user);
                }

                user.Email = email;
                user.Password = email.ToLower();
                user.IdNumber = item.IdNumber;
                user.FullName = item.FullName;
                user.Role = role;
                user.ClassId = role == DALEnums.Roles.Teacher ? null : classId;
                user.IsDeleted = false;
            }

            await dbContext.SaveChangesAsync();
            await usersService.ClearCacheAsync();
            return res.SetOk();
        }
    }
}
