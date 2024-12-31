using AutoMapper;
using FreeCourse.EventBus.Messages.Events;
using FreeCourse.Notification.API.Entities.Notification;
using FreeCourse.Notification.API.Hubs;
using FreeCourse.Notification.API.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Text.Json;

namespace FreeCourse.Notification.API.Consumer
{
    public class NotificationConsumer : IConsumer<NotificationEvent>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationEvent> _logger;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _notificationHub;
        public NotificationConsumer(
            IMapper mapper,
            ILogger<NotificationEvent> logger,
            INotificationService notificationService,
            IHubContext<NotificationHub> notificationHub)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationService = notificationService;
            _notificationHub = notificationHub;
        }


        public async Task Consume(ConsumeContext<NotificationEvent> context)
        {
          
            var notificationDto = _mapper.Map<NotificationDto>(context.Message);


            await _notificationService.Save(notificationDto);

            await _notificationService.GetNotificationsByUserId(notificationDto.UserId);

            //await _notificationHub.SendNotificationAll(notificationDto);
            await _notificationHub.Clients.All.SendAsync("ReceiveNotificationAll", JsonSerializer.Serialize(notificationDto)).ConfigureAwait(false);

            _logger.LogInformation("NotificationConsumer consumed successfully.Notification Id  : {notificitaionDto.Id}", notificationDto.Id);
        }
    }


}
