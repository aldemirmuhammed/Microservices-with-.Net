using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FreeCourse.Notification.API.Helpers;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using FreeCourse.Notification.API.Entities.Sms;
using FreeCourse.Notification.API.Services;
using FreeCourse.Notification.API.Services.Interfaces;
using FreeCourse.Shared.ControllerBases;

namespace Transflower.NotifiactionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : CustomBaseController
    {
        private readonly ISmsService _smsService;

        public SmsController(ISmsService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SmsMessage model)
        {
            return CreateActionResultInstance(await _smsService.SendSmsAsync(model));
        }
    }
}