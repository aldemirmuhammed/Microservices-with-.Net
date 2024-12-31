using FreeCourse.Notification.API.Entities.Email;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MimeKit;

namespace FreeCourse.Notification.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task<Response<EmailMessage>> SendEmailAsync(EmailMessage message);
    }
}
