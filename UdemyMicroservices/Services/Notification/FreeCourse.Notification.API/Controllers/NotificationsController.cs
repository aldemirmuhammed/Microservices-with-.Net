using FreeCourse.Notification.API.Entities.Notification;
using FreeCourse.Notification.API.Services.Interfaces;
using FreeCourse.Shared.ControllerBases;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Notification.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : CustomBaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
       

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            return CreateActionResultInstance(await _notificationService.GetNotificationById(id));
        }

        [HttpGet]
        [Route("/api/[controller]/GetNotificationsByUserId/{id}")]
        public async Task<IActionResult> GetNotificationsByUserId(string id)
        {
            //var usr = JsonSerializer.Deserialize<User>(user);
            return CreateActionResultInstance(await _notificationService.GetNotificationsByUserId(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return CreateActionResultInstance(await _notificationService.Delete(id));
        }


        [HttpPost]
        [Route("/api/[controller]/UpdateSeen/{id}")]
        public async Task<IActionResult> UpdateSeen(int id)
        {
            return CreateActionResultInstance(await _notificationService.UpdateSeen(id));
        }


        #region NotificationSettings

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdateSettings(NotificationSettings notificationSettings)
        {
            return CreateActionResultInstance(await _notificationService.SaveOrUpdateSettings(notificationSettings));
        }

        [HttpGet]
        [Route("/api/[controller]/GetNotificationsSetting/{id}")]
        public async Task<IActionResult> GetNotificationsSetting(string id)
        {
            return CreateActionResultInstance(await _notificationService.GetNotificationsSetting(id));
        }

        #endregion

    }
}
