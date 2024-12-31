using FreeCourse.Shared.Dtos;
using FreeCourse.Notification.API.Repositories.Interfaces;
using FreeCourse.Notification.API.Services.Interfaces;
using System.Collections.Generic;
using FreeCourse.Notification.API.Entities.Notification;

namespace FreeCourse.Notification.API.Services
{
    public class NotificationService : INotificationService
    {

        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Response<NotificationDto>> GetNotificationById(int notificationId)
        {

            var nDto = await _notificationRepository.GetNotificationById(notificationId);
            return Response<NotificationDto>.Success(nDto.Data, 200);
        }

        public async Task<Response<List<NotificationDto>>> GetNotificationsByUserId(string userId)
        {
            var notificationDtolist = await _notificationRepository.GetNotificationsByUserId(userId);

            if (notificationDtolist == null)
            {
                return Response<List<NotificationDto>>.Fail("Notifications not found", 404);
            }
            return Response<List<NotificationDto>>.Success(notificationDtolist.Data, 200);

        }

        public async Task<Response<NotificationDto>> Save(NotificationDto notificationDto)
        {
            await _notificationRepository.Save(notificationDto);
            return Response<NotificationDto>.Success(notificationDto, 200);
        }

        public async Task<Response<NoContent>> Delete(int notificationId)
        {
            await _notificationRepository.Delete(notificationId);
            return Response<NoContent>.Success(200);

        }

        public async Task<Response<NoContent>> UpdateSeen(int notificationId)
        {
            await _notificationRepository.UpdateSeen(notificationId);
            return Response<NoContent>.Success(200);

        }


        #region NotificationSetting


        public async Task<Response<NotificationSettings>> SaveOrUpdateSettings(NotificationSettings notificationSettings)
        {
            try
            {
                return await _notificationRepository.SaveorUpdateSettings(notificationSettings);
            }
            catch (Exception ex)
            { return Response<NotificationSettings>.Fail($"An error occurred while save or updating notifications setting. Err :{ex.Message}", 404); }
        }
        public async Task<Response<NotificationSettings>> GetNotificationsSetting(string id)
        {
            try
            {
                return await _notificationRepository.GetNotificationsSetting(id);
            }
            catch (Exception ex)
            { return Response<NotificationSettings>.Fail($"An error occurred while getting notifications setting. Err :{ex.Message}", 404); }

        }

        #endregion
    }
}
