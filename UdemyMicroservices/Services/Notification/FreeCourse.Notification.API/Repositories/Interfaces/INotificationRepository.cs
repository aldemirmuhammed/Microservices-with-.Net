using FreeCourse.Shared.Dtos;
using FreeCourse.Notification.API.Entities.Notification;

namespace FreeCourse.Notification.API.Repositories.Interfaces
{
    public interface INotificationRepository
    {

        #region Settings

        Task<Response<NotificationSettings>> SaveorUpdateSettings(NotificationSettings notificationSettings);

        Task<Response<NotificationSettings>> GetNotificationsSetting(string id);
        #endregion

        Task<Response<NotificationDto>> Save(NotificationDto notificationDto);

        Task<Response<List<NotificationDto>>> GetNotificationsByUserId(string userId);

        Task<Response<NotificationDto>> GetNotificationById(int notificationId);

        Task<Response<NotificationDto>> Delete(int notificationId);
        Task<Response<NotificationDto>> UpdateSeen(int notificationId);

    }
}
