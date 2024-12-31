using FreeCourse.EventBus.Messages.Events;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Services.Interfaces;
using FreeCourse.Shared.Messages;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Services
{
    public class MessageQueueService : IMessageQueueService
    {

        private readonly IPublishEndpoint _publishEndpoint;

        public MessageQueueService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task SendToBasket(string userId, string courseId, string updatedName)
        {
            try
            {
                await _publishEndpoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent
                {
                    UserId = userId,
                    CourseId = courseId,
                    UpdatedName = updatedName
                });

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task SendToNotification(string userId,string title,string message)
        {
            try
            {
                // send rabbit mq message to notification services
                await _publishEndpoint.Publish<NotificationEvent>(new NotificationEvent
                {
                    UserId = userId,
                    Title = $"{title}",
                    Message = $"{message}",
                    TargetSource = TargetSource.Course,
                    ExpiryAt = DateTime.Now.AddDays(1),
                    DisplayType = DisplayType.INBOX,
                    Severity = Severity.Information,
                    Status = "NEW"
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
