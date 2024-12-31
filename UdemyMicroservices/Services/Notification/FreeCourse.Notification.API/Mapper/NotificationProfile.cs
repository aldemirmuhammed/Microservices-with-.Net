using AutoMapper;
using FreeCourse.EventBus.Messages.Events;
using FreeCourse.Notification.API.Entities.Notification;
using FreeCourse.Shared.Messages;

namespace FreeCourse.Notification.API.Mapper
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationDto, NotificationEvent>().ReverseMap();
            CreateMap<CreateOrderMessageCommand,CreateOrderNotificationDto>().ReverseMap();
        }
    }
}
