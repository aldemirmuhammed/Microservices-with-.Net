using FreeCourse.Notification.API.Entities.Sms;
using FreeCourse.Notification.API.Helper.Sms;
using FreeCourse.Notification.API.Services.Interfaces;
using FreeCourse.Shared.Dtos;
using System.Diagnostics;
using Twilio;
using Twilio.Rest;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace FreeCourse.Notification.API.Services
{
    public class SmsService : ISmsService
    {
        private readonly SmsCredentials _credentials;

        public SmsService(SmsCredentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<Response<SmsMessage>> SendSmsAsync(SmsMessage smsMessage)
        {
            var accountSid = _credentials.SMSAccountIdentification;
            var authToken = _credentials.SMSAccountPassword;
            var fromNumber = _credentials.SMSAccountFrom;

            TwilioClient.Init(accountSid, authToken);

            MessageResource result = MessageResource.Create(
                new PhoneNumber(smsMessage.Phone),
                from: new PhoneNumber(fromNumber),
                body: smsMessage.Message);


            if (result.Status == MessageResource.StatusEnum.Queued)
            {
                return Response<SmsMessage>.Success(smsMessage, 200);
            }

            return Response<SmsMessage>.Fail(result.ErrorMessage, (int)result.ErrorCode);
        }
    }
}
