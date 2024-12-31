using FreeCourse.Notification.API.Entities.Email;
using FreeCourse.Notification.API.Services.Interfaces;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Notification.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : CustomBaseController
    {
        private readonly IEmailService _emailSender;

        public EmailController(IEmailService emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("/api/[controller]/SendMail")]

        public async Task<IActionResult> SendMail(EmailMessage emailMessage)
        {
            var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();
            if (files != null && files.Count > 0)
            {
                emailMessage.Attachments = files;
            }

            return CreateActionResultInstance(await _emailSender.SendEmailAsync(emailMessage));

        }
    }
}
