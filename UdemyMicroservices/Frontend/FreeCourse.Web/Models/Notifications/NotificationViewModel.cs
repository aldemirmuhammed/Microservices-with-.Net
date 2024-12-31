using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace FreeCourse.Web.Models.Notifications
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Status { get; set; } = null!;
        public Severity Severity { get; set; }
        public TargetSource TargetSource { get; set; }
        public DisplayType? DisplayType { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime? ExpiryAt { get; set; } = default(DateTime);

        public bool IsDeleted { get; set; } = false;
        public DateTime DeletedAt { get; set; } = default(DateTime);

        public bool IsSeen { get; set; } = false;
        public DateTime SeenAt { get; set; } = default(DateTime);

        public string ElapsedTime => GetElapsedTime(CreatedAt);



        public string GetElapsedTime(DateTime createdDate)
        {
            var dateTime = DateTime.Now.Subtract(createdDate);
            if (dateTime.TotalSeconds > 0 && dateTime.TotalSeconds < 60)
            {
                return $"{Convert.ToInt32(dateTime.TotalSeconds)} second ago";
            }
            else if (dateTime.TotalMinutes > 0 && dateTime.TotalMinutes < 60)
            {
                return $"{Convert.ToInt32(dateTime.TotalMinutes)} minutes ago";
            }
            else if (dateTime.TotalHours > 0 && dateTime.TotalHours <= 24)
            {
                return $"{Convert.ToInt32(dateTime.TotalHours)} hours ago";
            }
            else if (dateTime.TotalDays > 0 && dateTime.TotalDays <= 30)
            {
                return $"{Convert.ToInt32(dateTime.TotalDays)} days ago";
            }
            else if (dateTime.TotalDays > 31)
            {
                return $"{(Convert.ToInt32(dateTime.TotalDays) / 30)} months ago";
            }
            else
            {
                return $"{Convert.ToInt32(dateTime.TotalDays)} days ago";

            }
        }
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
        IdentityService,
        System


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
