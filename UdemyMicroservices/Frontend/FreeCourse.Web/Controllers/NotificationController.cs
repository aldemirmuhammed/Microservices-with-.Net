using FreeCourse.Web.Models.Notifications;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        public List<NotificationViewModel> NotificationList { get; set; } = new();

        private readonly INotificationService _notificationService;
        private readonly HubConnection _connection;
        private readonly ILogger<NotificationController> _logger;
        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;

            this.NotificationList = new List<NotificationViewModel>();

            _connection = new HubConnectionBuilder().WithUrl("http://localhost:5020/NotificationHub").Build();
            StartIfNeededAsync();
            _connection.Reconnecting += _connection_Reconnecting;
            _connection.Reconnected += _connection_Reconnected;
            _connection.Closed += _connection_Closed;

            //this.ViewData["NotificationList"] = this.NotificationList;
            ViewBag.NotificationListBag = NotificationList;
        }

        #region SignalR

        private async Task _connection_Closed(Exception arg)
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await StartIfNeededAsync();
            _logger.LogInformation("SignalR hub disconnected.");
            Console.WriteLine("SignalR hub disconnected.");

        }

        private Task _connection_Reconnected(string arg)
        {
            Console.WriteLine("SignalR hub reconnected.");
            _logger.LogInformation("SignalR hub reconnected.");

            return Task.CompletedTask;
        }

        private Task _connection_Reconnecting(Exception arg)
        {
            Console.WriteLine("SignalR hub reconnecting...");
            _logger.LogInformation("SignalR hub reconnecting.");

            return Task.CompletedTask;
        }
        public async Task StartIfNeededAsync()
        {
            _logger.LogInformation("SignalR hub no starting StartIfNeededAsync.");

            if (_connection.State == HubConnectionState.Disconnected)
            {
                _connection.On<string>("ReceiveNotificationAll", async (message) =>
                {
                    await GetMessages(message);
                    _logger.LogInformation("SignalR hub StartIfNeededAsync.");

                });

                await _connection.StartAsync();
            }
        }
        private async Task GetMessages(string message)
        {

            var notification = JsonSerializer.Deserialize<NotificationViewModel>(message);
            var list = await _notificationService.GetNotificationsByUserId();
            if (list == null)
                list = new List<NotificationViewModel>();
            ViewBag.NotificationListBag = list;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            var list = await _notificationService.GetNotificationsByUserId();
            if (list == null)
                list = new List<NotificationViewModel>();
            ViewBag.NotificationListBag = list;

            return View(list);
        }

        public async Task<IActionResult> GetNotificationById(int id)
        {
            var list = await _notificationService.GetNotificationById(id);
            if (list == null)
                list = new NotificationViewModel();
            ViewBag.NotificationListBag = list;
            return View(list);
        }

        public async Task<IActionResult> GetNotificationsByUserId()
        {
            var list = await _notificationService.GetNotificationsByUserId();
            if (list == null)
                list = new List<NotificationViewModel>();
            ViewBag.NotificationListBag = list;
            return View(NotificationList);
        }

        public async Task<IActionResult> Delete(int id)
        {

            await _notificationService.Delete(id);
            var list = await _notificationService.GetNotificationsByUserId();
            NotificationList = list;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAll()
        {
            var list = await _notificationService.GetNotificationsByUserId();
            if (list == null)
                return RedirectToAction(nameof(Index));

            foreach (var item in list)
            {
                await _notificationService.Delete(item.Id);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateSeen(int id)
        {

            await _notificationService.UpdateSeen(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateSeenAll()
        {
            var notificationList = await _notificationService.GetNotificationsByUserId();
            if (notificationList != null && notificationList.Count != 0)
            {
                foreach (var item in notificationList)
                {
                    await _notificationService.UpdateSeen(item.Id);
                }
            }
            NotificationList = notificationList;
            return RedirectToAction(nameof(Index));
        }

        #region NotificationsSettings

        public async Task<IActionResult> SaveOrUpdateSetting(bool isMute)
        {
            await _notificationService.SaveOrUpdateSetting(isMute);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetNotificationsSetting()
        {
            var settings = await _notificationService.GetNotificationsSetting();
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
