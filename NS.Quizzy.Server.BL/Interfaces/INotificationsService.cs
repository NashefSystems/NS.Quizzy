using NS.Quizzy.Server.BL.DTOs;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface INotificationsService : IBaseService<NotificationPayloadDto, NotificationDto>
    {
        Task<int> GetNumberOfMyNewNotificationsAsync();
        Task<List<MyNotificationItem>> GetMyNotificationsAsync(int? limit);
        Task<MyNotificationItem> MarkAsReadAsync(Guid notificationId);
    }
}
