using FreeCourse.Shared.Messages;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ChangeEmail;
using FreeCourse.Web.Models.Account.SendEmailConfirmation;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit.Text;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace FreeCourse.Web.Controllers.Account
{
    public class ChangeEmailController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        private readonly IEmailService _emailService;
        public ChangeEmailController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService, IEmailService emailService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData["Email"] = null;
            TempData["IsEmailConfirmed"] = null;

            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/ChangeEmail/Index.cshtml");

            var isConfirmed = await _ıdentityService.IsEmailConfirmedAsync(userId);
            if (isConfirmed.Data)
                TempData["IsEmailConfirmed"] = isConfirmed.Data;

            var getEmail = await _ıdentityService.GetEmailByUserId(userId);
            if (getEmail != null)
                TempData["Email"] = getEmail;

            return View("~/Views/Account/ChangeEmail/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel changeEmailViewModel)
        {
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null || changeEmailViewModel == null)
            {
                TempData["ChangeEmailStatusMessage"] = "Error Email change parameter has not empty";
                return View("~/Views/Account/ChangeEmail/Index.cshtml");
            }

            changeEmailViewModel.UserId = userId;
            if (changeEmailViewModel.NewEmail == null)
            {
                TempData["ChangeEmailStatusMessage"] = "Valid email error";
                return View("~/Views/Account/ChangeEmail/Index.cshtml");
            }
            var changeEmail = await _ıdentityService.ChangeEmailAsync(changeEmailViewModel);
            if (changeEmail.Data == null || changeEmail.Data.Token == null && changeEmail.Erros != null)
            {
                changeEmail.Erros.ForEach(x => TempData["ChangeEmailStatusMessage"] += x);
                return View("~/Views/Account/ChangeEmail/Index.cshtml");
            }


            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}";
            var callbackUrl = $"{baseUrl}/ConfirmEmailChange?id={changeEmailViewModel.UserId}&email={changeEmailViewModel.NewEmail}&token={changeEmail.Data.Token}";

            var message = new EmailMessage(new string[] { changeEmailViewModel.NewEmail }, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.", _textFormat: TextFormat.Html);
            var responseMsg = await _emailService.SendEmailAsync(message);

            if (responseMsg.Data == null && (responseMsg.Erros != null || responseMsg.Erros.Any()))
            {
                responseMsg.Erros.ForEach(x => TempData["ChangeEmailStatusMessage"] += x);
                return View("~/Views/Account/ChangeEmail/Index.cshtml");
            }

            TempData["ChangeEmailStatusMessage"] = "Success Confirmation link to change email sent. Please check your email.";

            return View("~/Views/Account/ChangeEmail/Index.cshtml");
        }


        [HttpPost]
        public async Task<IActionResult> SendVerificationEmail()
        {

            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
            {
                TempData["ChangeEmailStatusMessage"] = "Error Email change parameter has not empty";
                return View("~/Views/Account/ChangeEmail/Index.cshtml");
            }

            var email = await _ıdentityService.GetEmailByUserId(userId);
            if (email == null)
            {
                TempData["ChangeEmailStatusMessage"] = "Error Email change parameter has not empty";
                return View("~/Views/Account/ChangeEmail/Index.cshtml");
            }

            SendEmailConfirmationViewModel sendEmailConfirmationViewModel = new SendEmailConfirmationViewModel
            {
                Email = email,
                UserId = userId
            };

            var sendEmailVerification = await _ıdentityService.SendVerificationEmailAsync(sendEmailConfirmationViewModel);
            if (sendEmailVerification.Data == null && sendEmailVerification.Erros != null)
            {
                sendEmailVerification.Erros.ForEach(x => TempData["ChangeEmailStatusMessage"] += x);
                return View("~/Views/Account/ChangeEmail/Index.cshtml");
            }

            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}";

            var callbackUrl = $"{baseUrl}/ConfirmEmail?id={sendEmailVerification.Data.UserId}&token={sendEmailVerification.Data.Code}";
            var message = new EmailMessage(new string[] { sendEmailConfirmationViewModel.Email }, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.", _textFormat: TextFormat.Html);
            var responseMsg = await _emailService.SendEmailAsync(message);

            if (responseMsg.Data == null && responseMsg.Erros != null)
            {
                responseMsg.Erros.ForEach(x => TempData["ChangeEmailStatusMessage"] += x);
                return View("~/Views/Account/ChangeEmail/Index.cshtml");
            }

            TempData["ChangeEmailStatusMessage"] = "Success Verification email sent. Please check your email.";

            return View("~/Views/Account/ChangeEmail/Index.cshtml");
        }
    }
}
