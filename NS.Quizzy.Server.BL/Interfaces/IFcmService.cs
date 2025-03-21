using NS.Quizzy.Server.BL.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IFcmService
    {
        Task SendPushNotificationAsync(PushNotificationRequest request);
    }
}
