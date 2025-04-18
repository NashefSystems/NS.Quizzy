using NS.Quizzy.Server.BL.DTOs;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface INotificationsService : IBaseService<NotificationPayloadDto, NotificationDto>
    {
        Task<List<NotificationDto>> GetMyNotificationsAsync(bool isArchive);
    }
}
