using FreeCourse.Web.Models.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationViewModel> GetNotificationById(int id);

        Task<List<NotificationViewModel>> GetNotificationsByUserId();

        Task<bool> Delete(int id);

        Task<bool> UpdateSeen(int id);

        Task<bool> SaveOrUpdateSetting(bool isMute);

        Task<NotificationSettingsViewModel> GetNotificationsSetting();

    }
}
