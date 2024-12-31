using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User.Management.API.Dtos.ChangeEmail;
using User.Management.API.Dtos.ChangePassword;
using User.Management.API.Dtos.ConfirmEmail;
using User.Management.API.Dtos.ConfirmEmailChange;
using User.Management.API.Dtos.EnableAuthenticator;
using User.Management.API.Dtos.ExternalLogins;
using User.Management.API.Dtos.ForgotPassword;
using User.Management.API.Dtos.Login;
using User.Management.API.Dtos.PersonalData;
using User.Management.API.Dtos.Register;
using User.Management.API.Dtos.ResetPassword;
using User.Management.API.Dtos.SendEmailConfirmation;
using User.Management.API.Models;
using User.Management.API.Services.Interfaces;


namespace User.Management.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : CustomBaseController
    {


        private readonly IUserManagement _user;
        private readonly IConfiguration _configuration;

        public AccountController(IUserManagement user, IConfiguration configuration)
        {
            _user = user;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("/api/[controller]/GetEmailByUserId/{userId}")]
        public async Task<IActionResult> GetEmailByUserId(string userId)
        {
            var user = await _user.FindByIdAsync(userId);
            if (user != null && user.Email != null)
            {
                return CreateActionResultInstance(Response<string>.Success(user.Email, 200));
            }
            return CreateActionResultInstance(Response<string>.Fail("This user email doesnot exist error!", 404));
        }

        #region Register

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerUser)
        {
            return CreateActionResultInstance(await _user.RegisterAsync(registerUser));
        }


        [HttpPost]
        [Route("generate-email-confirmation-token")]
        public async Task<IActionResult> GenerateEmailConfirmationToken(RegisterConfirmationDto registerConfirmationDto)
        {
            return CreateActionResultInstance(await _user.GenerateEmailConfirmationTokenAsync(registerConfirmationDto));
        }

        #endregion

        #region Login

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto loginModel)
        {
            return CreateActionResultInstance(await _user.LoginAsync(loginModel));
        }


        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string userName)
        {
            return CreateActionResultInstance(await _user.LoginWithOTPAsync(code, userName));

        }


        [HttpPost]
        [Route("Refresh-Token")]
        public async Task<IActionResult> RefreshToken(LoginResponseDto tokens)
        {
            return CreateActionResultInstance(await _user.RefreshTokenAsync(tokens));

        }

        #endregion

        #region ResetPassword

        [HttpPost]
        [Route("SendResetPassword")]
        public async Task<IActionResult> SendResetPassword(ForgotPasswordDto forgotPassword)
        {
            return CreateActionResultInstance(await _user.SendResetPasswordAsync(forgotPassword));
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            return CreateActionResultInstance(await _user.ResetPasswordAsync(resetPasswordDto));
        }

        #endregion

        #region ChangePassword

        [HttpGet]
        [Route("/api/[controller]/HasPasswordAsync/{userId}")]
        public async Task<IActionResult> HasPasswordAsync(string userId)
        {
            return CreateActionResultInstance(await _user.HasPasswordAsync(userId));
        }

        [HttpPost]
        [Route("ChangePasswordAsync")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            return CreateActionResultInstance(await _user.ChangePasswordAsync(changePasswordDto));
        }


        [HttpPost]
        [Route("SetPasswordAsync")]
        public async Task<IActionResult> SetPasswordAsync(SetPasswordDto setPasswordDto)
        {
            return CreateActionResultInstance(await _user.SetPasswordAsync(setPasswordDto));
        }

        #endregion

        #region PersonalData

        [HttpPost]
        [Route("DeletePersonalDataAsync")]
        public async Task<IActionResult> DeletePersonalDataAsync(DeletePersonalDataDto deletePersonalDataDto)
        {
            return CreateActionResultInstance(await _user.DeletePersonalDataAsync(deletePersonalDataDto));
        }


        [HttpPost]
        [Route("DownloadPersonalDataAsync")]
        public async Task<IActionResult> DownloadPersonalDataAsync(DownloadPersonalDataDto downloadPersonalDataDto)
        {
            return CreateActionResultInstance(await _user.DownloadPersonalDataAsync(downloadPersonalDataDto));
        }

        #endregion

        #region Disable2fa

        [HttpGet]
        [Route("/api/[controller]/GetTwoFactorEnabledAsync/{userId}")]
        public async Task<IActionResult> GetTwoFactorEnabledAsync(string userId)
        {
            return CreateActionResultInstance(await _user.GetTwoFactorEnabledAsync(userId));
        }

        [HttpGet]
        [Route("/api/[controller]/SetTwoFactorEnabledAsync/{userId}")]
        public async Task<IActionResult> SetTwoFactorEnabledAsync(string userId)
        {
            return CreateActionResultInstance(await _user.SetTwoFactorEnabledAsync(userId));
        }

        #endregion

        #region ChangeEmail

        [HttpGet]
        [Route("/api/[controller]/IsEmailConfirmed/{userId}")]
        public async Task<IActionResult> IsEmailConfirmed(string userId)
        {
            return CreateActionResultInstance(await _user.IsEmailConfirmedAsync(userId));
        }

        [HttpPost]
        [Route("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailDto changeEmailDto)
        {
            return CreateActionResultInstance(await _user.ChangeEmailAsync(changeEmailDto));
        }

        [HttpPost]
        [Route("SendVerificationEmail")]
        public async Task<IActionResult> SendVerificationEmail(SendEmailConfirmationDto sendEmailConfirmationDto)
        {
            return CreateActionResultInstance(await _user.SendVerificationEmailAsync(sendEmailConfirmationDto));
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            return CreateActionResultInstance(await _user.ConfirmEmailAsync(confirmEmailDto));
        }

        [HttpPost]
        [Route("ConfirmEmailChange")]
        public async Task<IActionResult> ConfirmEmailChange(ConfirmEmailChangeDto confirmEmailChangeDto)
        {
            return CreateActionResultInstance(await _user.ConfirmEmailChangeAsync(confirmEmailChangeDto));
        }


        #endregion

        #region EnableAuthenticator

        [HttpGet]
        [Route("/api/[controller]/LoadSharedKeyAndQrCodeUriAsync/{userId}")]
        public async Task<IActionResult> LoadSharedKeyAndQrCodeUriAsync(string userId)
        {
            return CreateActionResultInstance(await _user.EnableAuthenticatorLoadSharedKeyAndQrCodeUriAsync(userId));
        }

        [HttpPost]
        [Route("VerifyAsync")]
        public async Task<IActionResult> VerifyAsync(EnableAuthenticatorDto enableAuthenticatorDto)
        {
            return CreateActionResultInstance(await _user.VerifyAsync(enableAuthenticatorDto));
        }


        #endregion

        #region TwoFactorAuthentication

        [HttpGet]
        [Route("/api/[controller]/GetTwoFactorAuthentication/{userId}")]
        public async Task<IActionResult> GetTwoFactorAuthentication(string userId)
        {
            return CreateActionResultInstance(await _user.GetTwoFactorAuthenticationAsync(userId));
        }

        [HttpPost]
        [Route("ForgetTwoFactorClient")]
        public async Task<IActionResult> ForgetTwoFactorClient(string userId)
        {
            return CreateActionResultInstance(await _user.ForgetTwoFactorClientAsync(userId));
        }


        #endregion

        #region ResetAuthenticator

        [HttpPost]
        [Route("ResetAuthenticator")]
        public async Task<IActionResult> ResetAuthenticator(string userId)
        {
            return CreateActionResultInstance(await _user.ResetAuthenticatorAsync(userId));
        }

        #endregion

        #region GenerateRecoveryCodes

        [HttpGet]
        [Route("/api/[controller]/GenerateRecoveryCodes/{userId}")]
        public async Task<IActionResult> GenerateRecoveryCodes(string userId)
        {
            return CreateActionResultInstance(await _user.GenerateRecoveryCodesAsync(userId));
        }

        #endregion

        #region ExternalLogins

        [HttpGet]
        [Route("/api/[controller]/GetExternalLogins/{userId}")]
        public async Task<IActionResult> GetExternalLogins(string userId)
        {
            return CreateActionResultInstance(await _user.GetExternalLoginsAsync(userId));
        }

        [HttpPost]
        [Route("RemoveExternalLogins")]
        public async Task<IActionResult> RemoveExternalLogins(RemoveExternalLoginDto removeExternalLoginDto)
        {
            return CreateActionResultInstance(await _user.RemoveExternalLoginsAsync(removeExternalLoginDto));
        }

        [HttpPost]
        [Route("LinkExternalLogins")]
        public async Task<IActionResult> LinkExternalLogins(LinkExternalLoginDto linkExternalLoginDto)
        {
            return CreateActionResultInstance(await _user.LinkExternalLoginsAsync(linkExternalLoginDto));
        }

        [HttpGet]
        [Route("/api/[controller]/LinkLoginCallback/{userId}")]
        public async Task<IActionResult> LinkLoginCallback(string userId)
        {
            return CreateActionResultInstance(await _user.LinkLoginCallbackAsync(userId));
        }

        #endregion

        #region UserProfile

        [HttpGet]
        [Route("/api/[controller]/GetUserProfile/{userId}")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            return CreateActionResultInstance(await _user.GetUserProfileAsync(userId));
        }


        [HttpPost]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(ApplicationUser applicationUser)
        {
            return CreateActionResultInstance(await _user.UpdateProfileAsync(applicationUser));
        }
        #endregion
    }
}
