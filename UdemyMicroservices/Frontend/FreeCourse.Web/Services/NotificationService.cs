using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.Discount;
using FreeCourse.Web.Models.Notifications;
using FreeCourse.Web.Services.Interfaces;
using NuGet.Protocol;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class NotificationService : INotificationService
    {

        #region fields


        private static Dictionary<string, List<string>> userConnectionMap = new Dictionary<string, List<string>>();
        private static string userConnectionMapLocker = string.Empty;

        #endregion

        private readonly HttpClient _httpClient;
        private readonly ISharedIdentityService _sharedIdentityService;

        public NotificationService(HttpClient httpClient, ISharedIdentityService sharedIdentityService)
        {
            _httpClient = httpClient;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<NotificationViewModel> GetNotificationById(int id)
        {
            var response = await _httpClient.GetAsync($"notifications/GetNotificationById/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var notification = await response.Content.ReadFromJsonAsync<Response<NotificationViewModel>>();
            return notification.Data;
        }

        public async Task<List<NotificationViewModel>> GetNotificationsByUserId()
        {
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return null;
            var response = await _httpClient.GetAsync($"notifications/GetNotificationsByUserId/{userId}");

            if (!response.IsSuccessStatusCode)
                return null;
            var notification = await response.Content.ReadFromJsonAsync<Response<List<NotificationViewModel>>>().ConfigureAwait(false);
            return notification.Data;
        }


        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"notifications/{id}");

            var notification = await response.Content.ReadFromJsonAsync<Response<List<NotificationViewModel>>>();
            return notification.IsSuccessful;
        }

        public async Task<bool> UpdateSeen(int id)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"notifications/UpdateSeen/{id}", id);
                //var notification = await response.Content.ReadFromJsonAsync<Response<NoContent>>();
                return response.IsSuccessStatusCode;
            }
            catch
            { return false; }
        }


        #region NotificationSettings

        public async Task<NotificationSettingsViewModel> GetNotificationsSetting()
        {
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return null;
            var response = await _httpClient.GetAsync($"notifications/GetNotificationsSetting/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var notification = await response.Content.ReadFromJsonAsync<Response<NotificationSettingsViewModel>>().ConfigureAwait(false);
            return notification.Data;
        }


        public async Task<bool> SaveOrUpdateSetting(bool isMute)
        {
            try
            {
                var notificationSettings = new NotificationSettingsViewModel
                {
                    IsNotificationMute = isMute,
                    UserId = _sharedIdentityService.GetUserId,
                    NotificationMuteAt = isMute == true ? DateTime.Now : default(DateTime)
                };

                var response = await _httpClient.PostAsJsonAsync<NotificationSettingsViewModel>($"notifications", notificationSettings);
                //var notification = await response.Content.ReadFromJsonAsync<Response<NotificationSettingsViewModel>>();
                return response.IsSuccessStatusCode;
            }
            catch
            { return false; }

        }

        #endregion
    }
}
