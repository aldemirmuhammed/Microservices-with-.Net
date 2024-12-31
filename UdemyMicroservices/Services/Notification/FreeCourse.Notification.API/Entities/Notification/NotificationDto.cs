using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreeCourse.Notification.API.Entities.Notification
{
    [Table("Notifications")]
    public class NotificationDto /*: NotificationData*/
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Status { get; set; } = null!;
        public Severity Severity { get; set; }
        public TargetSource TargetSource { get; set; }
        public DisplayType? DisplayType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ExpiryAt { get; set; } = default(DateTime);

        public bool IsDeleted { get; set; } = false;
        public DateTime DeletedAt { get; set; } = default;

        public bool IsSeen { get; set; } = false;
        public DateTime SeenAt { get; set; } = default;
    }


    public enum TargetSource
    {
        Gateway,
        Basket,
        Course,
        Order,
        PhotoStock,
        Discount,
        Web,
        IdentityService

    }

    public enum DisplayType
    {
        INBOX,
        OUTBOX

    }

    public enum Severity
    {

        Error,
        Warning,
        Information,


    }
}
