using FreeCourse.Notification.API.Entities.Sms;
using FreeCourse.Shared.Dtos;
using Twilio.TwiML.Messaging;

namespace FreeCourse.Notification.API.Services.Interfaces
{
    public interface ISmsService
    {

        Task<Response<SmsMessage>> SendSmsAsync(SmsMessage smsMessage);
    }
}
