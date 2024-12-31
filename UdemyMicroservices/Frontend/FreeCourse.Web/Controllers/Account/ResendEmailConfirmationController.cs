using FreeCourse.Shared.Messages;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ChangeEmail;
using FreeCourse.Web.Models.Account.SendEmailConfirmation;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit.Text;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class ResendEmailConfirmationController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        private readonly IEmailService _emailService;
        public ResendEmailConfirmationController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService, IEmailService emailService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData["ResendEmailConfirmationMessage"] = null;

            return View("~/Views/Account/ResendEmailConfirmation/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(SendEmailConfirmationViewModel sendEmailConfirmationViewModel)
        {
            TempData["ResendEmailConfirmationMessage"] = null;


            sendEmailConfirmationViewModel.UserId = _sharedIdentityService.GetUserId;
            var sendEmailVerification = await _ıdentityService.SendVerificationEmailAsync(sendEmailConfirmationViewModel);
            if (sendEmailVerification.Data == null || sendEmailVerification.Erros != null)
            {
                sendEmailVerification.Erros.ForEach(x => TempData["ResendEmailConfirmationMessage"] += x);
                return View("~/Views/Account/ResendEmailConfirmation/Index.cshtml");
            }
            //var encodeUserId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(sendEmailVerification.Data.UserId));
            //var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(sendEmailVerification.Data.Code));

            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}";
            var callbackUrl = $"{baseUrl}/ConfirmEmail?id={sendEmailConfirmationViewModel.UserId}&token={sendEmailConfirmationViewModel.Code}";
           
            var message = new EmailMessage(new string[] { sendEmailConfirmationViewModel.Email }, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.", _textFormat: TextFormat.Html);
            var responseMsg = await _emailService.SendEmailAsync(message);

            if (responseMsg.Data == null || responseMsg.Erros != null)
            {
                responseMsg.Erros.ForEach(x => { ModelState.AddModelError(string.Empty, x); });
                return View("~/Views/Account/ResendEmailConfirmation/Index.cshtml");
            }

            TempData["ResendEmailConfirmationMessage"] = "Success Verification email sent. Please check your email.";

            return View("~/Views/Account/ResendEmailConfirmation/Index.cshtml");
        }
    }
}
