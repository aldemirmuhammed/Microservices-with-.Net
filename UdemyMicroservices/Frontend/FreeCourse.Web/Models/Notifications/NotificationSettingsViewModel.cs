using System;

namespace FreeCourse.Web.Models.Notifications
{
    public class NotificationSettingsViewModel
    {

        public int Id { get; set; }
        public string UserId { get; set; } = null!;

        public bool IsNotificationMute { get; set; } = false;

        public DateTime NotificationMuteAt { get; set; }


    }
}
