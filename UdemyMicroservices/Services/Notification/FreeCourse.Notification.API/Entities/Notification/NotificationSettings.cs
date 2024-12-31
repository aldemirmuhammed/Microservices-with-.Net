using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreeCourse.Notification.API.Entities.Notification
{
    [Table("NotificationSetting")]
    public class NotificationSettings
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = null!;

        public bool IsNotificationMute { get; set; } = false;

        public DateTime NotificationMuteAt { get; set; }


    }
}
