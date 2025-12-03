using NS.Quizzy.Server.BL.Interfaces;
using NS.Shared.QueueManager.Interfaces;

namespace NS.Quizzy.Server.BL.Services
{
    internal class SystemService : ISystemService
    {
        private readonly INSQueueService _queueService;

        public SystemService(INSQueueService queueService)
        {
            _queueService = queueService;
        }

        public async Task<object> ReQueueDlqMessagesAsync(string queueName)
        {
            var res = await _queueService.ReQueueDlqMessagesAsync(queueName, BLConsts.QUEUE_VIRTUAL_HOST);
            return res;
        }
    }
}
