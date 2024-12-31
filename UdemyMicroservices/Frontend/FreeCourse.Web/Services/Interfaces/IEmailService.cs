using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IEmailService
    {
        Task<Response<EmailMessage>> SendEmailAsync(EmailMessage message);

    }
}
