using AutoMapper;
using FreeCourse.Shared.Messages;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ForgotPassword;
using FreeCourse.Web.Models.Account.Register;
using FreeCourse.Web.Models.Account.ResetPassword;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit.Text;
using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        private readonly IEmailService _emailService;
        public AccountController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService, IEmailService emailService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationViewModel userModel)
        {
            var phone = userModel.FieldCode + userModel.Phone;
            userModel.Phone = phone;
            userModel.Roles = new System.Collections.Generic.List<string>();
            userModel.Roles.Add("User");
            var result = await _ıdentityService.RegisterAsync(userModel);
            if (result.Data == null || result.Data.Token == null || (result.Erros != null && result.Erros.Any()))
            {
                result.Erros.ForEach(x => { ModelState.AddModelError(string.Empty, x); });
                return View();
            }

            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}";
            var callbackUrl = $"{baseUrl}/Account/ConfirmEmail?id={result.Data.User.Id}&token={result.Data.Token}";

            var message = new EmailMessage(new string[] { result.Data.User.Email! }, "Confirmation email link", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.", _textFormat: TextFormat.Html);
            var responseMsg = _emailService.SendEmailAsync(message);

            if (result.Data.RequireConfirmedAccount)
                return RedirectToAction(nameof(Index), "RegisterConfirmation", new { email = result.Data.User.Email, returnUrl = "/Auth/SignIn" });
            return RedirectToAction(nameof(SignIn), "Auth");
        }

        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (forgotPasswordViewModel == null || forgotPasswordViewModel.Email == null)
                return View();
            var result = await _ıdentityService.ForgotPasswordAsync(forgotPasswordViewModel);
            if (result.Data != null && result.Data.Code != null && (result.Erros == null || !result.Erros.Any()))
            {
                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}";
                var callbackUrl = $"{baseUrl}/ResetPassword?id={result.Data.UserId}&token={result.Data.Code}";

                var message = new EmailMessage(new string[] { forgotPasswordViewModel.Email! }, "Reset Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.\"", _textFormat: TextFormat.Html);
                var responseMsg = _emailService.SendEmailAsync(message);

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            return View();
        }

        public async Task<IActionResult> ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> ResetPassword([FromQuery] string id, [FromQuery] string token, ResetPasswordViewModel resetPasswordViewModel)
        {
            if (id == null || token == null)
                return BadRequest();


            //var UserMail = await _ıdentityService.GetEmailByUserId(userId);
            //var Token = _token;
            //if (UserMail == null || Token == null)
            //    return BadRequest();
            if (resetPasswordViewModel == null || resetPasswordViewModel.Password == null || resetPasswordViewModel.ConfirmPassword == null)
                return View();

            if (resetPasswordViewModel.Email == null)
                resetPasswordViewModel.Email = "Temp";
            resetPasswordViewModel.Code = token;
            resetPasswordViewModel.UserId = id;
            var result = await _ıdentityService.ResetPasswordAsync(resetPasswordViewModel);
            if (result.Data)
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            return View();
        }

        public async Task<IActionResult> ResetPasswordConfirmation()
        {
            return View();
        }



    }
}
