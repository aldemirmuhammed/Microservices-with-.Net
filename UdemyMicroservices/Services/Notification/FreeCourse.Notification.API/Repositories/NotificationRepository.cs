using FreeCourse.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using FreeCourse.Notification.API.Repositories.Interfaces;
using System.Collections.Generic;
using FreeCourse.Notification.API.Entities.Notification;

namespace FreeCourse.Notification.API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {

        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<NotificationDto>> Delete(int notificationId)
        {
            //var notificationDto = await GetNotificationById(notificationId);
            var notificationItem = await _context.Notifications.Where(x => x.Id == notificationId).FirstOrDefaultAsync();
            if (notificationItem == null)
                return Response<NotificationDto>.Fail("Notification not found for deleted", 404);
            notificationItem.IsDeleted = true;
            notificationItem.DeletedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Response<NotificationDto>.Success(notificationItem, 200);
        }
        public async Task<Response<NotificationDto>> UpdateSeen(int notificationId)
        {
            var notificationDto = await _context.Notifications.Where(x => x.Id == notificationId).FirstOrDefaultAsync();
            if (notificationDto != null)
            {
                if (notificationDto.IsSeen)
                {
                    return Response<NotificationDto>.Fail("Notification already updated", 404);

                }
                notificationDto.IsSeen = true;
                notificationDto.SeenAt = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                _context.Notifications.Update(notificationDto);
                await _context.SaveChangesAsync();

                return Response<NotificationDto>.Success(notificationDto, 200);

            }
            return Response<NotificationDto>.Fail("Notification couldnt find", 404);
        }

        public async Task<Response<NotificationDto>> GetNotificationById(int notificationId)
        {
            var item = await _context.Notifications.Where(x => x.Id == notificationId).FirstOrDefaultAsync();
            if (item == null)
            {
                return Response<NotificationDto>.Fail("Notifications not found ", 404);
            }
            return Response<NotificationDto>.Success(item, 200);


        }

        // get notifications as isdeleted ad user ıd 
        public async Task<Response<List<NotificationDto>>> GetNotificationsByUserId(string userId)
        {
            var notificationSettings = await _context.NotificationSettings.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (notificationSettings != null)
            {
                var notificationDtolist = await _context.Notifications.Where(x => x.UserId == userId && x.IsDeleted == false).ToListAsync();
                if (notificationDtolist != null)
                {
                    if (!notificationSettings.IsNotificationMute)
                    {
                        return Response<List<NotificationDto>>.Success(notificationDtolist, 200);
                    }
                    else
                    {
                        var list = notificationDtolist.Where(x => x.CreatedAt < notificationSettings.NotificationMuteAt).ToList();

                        return Response<List<NotificationDto>>.Success(list, 200);

                    }
                }
            }

            return Response<List<NotificationDto>>.Fail("Users notifications not found", 404);

        }

        public async Task<Response<NotificationDto>> Save(NotificationDto notificationDto)
        {
            try
            {
                await _context.Notifications.AddAsync(notificationDto);
                await _context.SaveChangesAsync();
                return Response<NotificationDto>.Success(notificationDto, 200);
            }
            catch (Exception ex)
            { return Response<NotificationDto>.Fail($"An error occurred while saving notification. Err : {ex.Message}", 404); }
        }


        #region NotificationsSettings

        public async Task<Response<NotificationSettings>> SaveorUpdateSettings(NotificationSettings notificationSettings)
        {
            try
            {
                var isExist = await _context.NotificationSettings.Where(x => x.UserId == notificationSettings.UserId).FirstOrDefaultAsync();
                if (isExist != null)
                {
                    isExist.NotificationMuteAt = notificationSettings.NotificationMuteAt;
                    isExist.IsNotificationMute = notificationSettings.IsNotificationMute;
                    _context.NotificationSettings.Update(isExist);
                    await _context.SaveChangesAsync();
                    return Response<NotificationSettings>.Success(isExist, 200);
                }

                await _context.NotificationSettings.AddAsync(notificationSettings);
                await _context.SaveChangesAsync();
                return Response<NotificationSettings>.Success(notificationSettings, 200);
            }
            catch (Exception ex)
            { return Response<NotificationSettings>.Fail($"An error occurred while saving notification settings. Err : {ex.Message}", 404); }

        }


        public async Task<Response<NotificationSettings>> GetNotificationsSetting(string id)
        {
            try
            {
                var notificationSeetings = await _context.NotificationSettings.Where(
                x => x.UserId == id).FirstOrDefaultAsync();
                if (notificationSeetings == null)
                {
                    return Response<NotificationSettings>.Fail("Notifications settings not found as user ıd", 404);
                }
                return Response<NotificationSettings>.Success(notificationSeetings, 200);

            }
            catch (Exception ex)
            { return Response<NotificationSettings>.Fail($"An error occurred while getting notification settings. Err : {ex.Message}", 404); }
        }

        #endregion
    }
}
