using FreeCourse.Shared.Dtos;
using FreeCourse.Notification.API.Entities.Notification;

namespace FreeCourse.Notification.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task<Response<NotificationSettings>> SaveOrUpdateSettings(NotificationSettings notificationSettings);

        Task<Response<NotificationSettings>> GetNotificationsSetting(string id);

        Task<Response<NotificationDto>> Save(NotificationDto notificationDto);

        Task<Response<List<NotificationDto>>> GetNotificationsByUserId(string userId);

        Task<Response<NotificationDto>> GetNotificationById(int notificationId);

        Task<Response<NoContent>> Delete(int notificationId);

        Task<Response<NoContent>> UpdateSeen(int notificationId);
    }
}
