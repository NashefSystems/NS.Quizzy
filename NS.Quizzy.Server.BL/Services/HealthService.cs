using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.DAL;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.QueueManager.Interfaces;

namespace NS.Quizzy.Server.BL.Services
{
    internal class HealthService : IHealthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly INSCacheProvider _cacheProvider;
        private readonly INSQueueService _queueService;

        public HealthService(AppDbContext appDbContext, INSCacheProvider cacheProvider, INSQueueService queueService)
        {
            _appDbContext = appDbContext;
            _cacheProvider = cacheProvider;
            _queueService = queueService;
        }

        public async Task<ReadinessResult> ReadinessAsync()
        {
            var dataBaseCheckTask = DataBaseCheckAsync();
            var rabbitMQCheckTask = RabbitMQCheckAsync();
            var redisCheckTask = RedisCheckAsync();
            await Task.WhenAll(dataBaseCheckTask, rabbitMQCheckTask, redisCheckTask);

            var result = new ReadinessResult
            {
                Checks = new Dictionary<string, HealthCheckResult>
                {
                    { "DataBase", dataBaseCheckTask.Result},
                    { "Redis", redisCheckTask.Result},
                    { "RabbitMQ", rabbitMQCheckTask.Result},
                }
            };
            result.Status = result.Checks.Values.Min(x => x.Status);
            return result;
        }

        private async Task<HealthCheckResult> DataBaseCheckAsync()
        {
            try
            {
                await _appDbContext.Database.ExecuteSqlRawAsync("SELECT 1;");
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }

        private async Task<HealthCheckResult> RabbitMQCheckAsync()
        {
            try
            {
                return await _queueService.HealthCheckAsync();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Degraded(exception: ex);
            }
        }

        private async Task<HealthCheckResult> RedisCheckAsync()
        {
            try
            {
                var keys = await _cacheProvider.GetKeysAsync();
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}
