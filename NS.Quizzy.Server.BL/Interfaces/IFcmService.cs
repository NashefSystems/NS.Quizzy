using NS.Quizzy.Server.BL.Models;
using NS.Shared.Logging;

namespace NS.Quizzy.Server.BL.Interfaces
{
    internal interface IFcmService
    {
        Task<bool> SendPushNotificationAsync(PushNotificationRequest request, INSLogBag parentLogBag = null);
    }
}
