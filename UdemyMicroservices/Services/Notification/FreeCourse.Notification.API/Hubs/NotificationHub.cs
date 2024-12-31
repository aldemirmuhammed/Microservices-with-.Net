using FreeCourse.Notification.API.Entities.Notification;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
namespace FreeCourse.Notification.API.Hubs
{
    public class NotificationHub : Hub
    {
        private Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        public override Task OnConnectedAsync()
        {
            string? value;
            keyValuePairs.TryGetValue(Context.ConnectionId,out value);
            if (value == null)
            {
                keyValuePairs.Add(Context.ConnectionId, value);
            }
            //get conntection Id Context.ConnectionId;
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
             string? value;
            keyValuePairs.TryGetValue(Context.ConnectionId, out value);
            if (value != null)
            {
                keyValuePairs.Remove(Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotificationAll", message);
        }


        public async Task SendNotificationAll(NotificationDto notificationDto)
        {
            notificationDto.CreatedAt = DateTime.Parse(notificationDto.CreatedAt.ToString("dd/MM/yyyy"));
            string? value;
            keyValuePairs.TryGetValue(Context.ConnectionId, out value);
            if (value != null)
            {
                
                await Clients.All.SendAsync("ReceiveNotificationAll", JsonSerializer.Serialize(notificationDto));

            }
        }

        public async Task SendNotificationByUserId(string userId, NotificationDto notificationDto)
        {
            await Clients.Clients(userId).SendAsync("ReceiveNotificationByUserId", notificationDto);
        }

        public async Task SendNotificationByUserIdList(List<string> userIdList, NotificationDto notificationDto)
        {
            foreach (var item in userIdList)
            {
                await Clients.Clients(item).SendAsync("ReceiveNotificationByUserIdList", notificationDto);

            }
        }
    }
}
