using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.EventBus.Messages.Common
{
    public static class EventBusConstants
    {
        public const string CreateOrderQueue = "create-order-service";
        public const string CourseNameChangedEventOrder = "course-name-changed-event-order-service";
        public const string NotificationQueue = "notification-queue";
    }
}
