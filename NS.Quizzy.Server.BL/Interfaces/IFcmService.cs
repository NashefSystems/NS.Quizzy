using NS.Quizzy.Server.BL.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    internal interface IFcmService
    {
        Task<bool> SendPushNotificationAsync(PushNotificationRequest request);
    }
}
