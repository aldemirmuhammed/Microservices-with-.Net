using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Account;
using FreeCourse.Web.Models.Account.ChangeEmail;
using FreeCourse.Web.Models.Account.ChangePassword;
using FreeCourse.Web.Models.Account.ConfirmEmail;
using FreeCourse.Web.Models.Account.ConfirmEmailChange;
using FreeCourse.Web.Models.Account.EnableAuthenticator;
using FreeCourse.Web.Models.Account.ExternalLogins;
using FreeCourse.Web.Models.Account.ForgotPassword;
using FreeCourse.Web.Models.Account.GenerateRecoveryCodes;
using FreeCourse.Web.Models.Account.PersonalData;
using FreeCourse.Web.Models.Account.Register;
using FreeCourse.Web.Models.Account.RegisterConfirmation;
using FreeCourse.Web.Models.Account.ResetPassword;
using FreeCourse.Web.Models.Account.SendEmailConfirmation;
using FreeCourse.Web.Models.Account.SignIn;
using FreeCourse.Web.Models.Account.TwoFactorAuthentication;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IIdentityService
    {
        #region SignIn

        Task<Response<bool>> SignIn(SigninInput signInInput);

        Task<Response<SigninInput>> LoginAsync(SigninInput signInInput);

        #endregion

        #region Token

        Task<TokenResponse> GetAccessTokenByRefreshToken();

        Task RevokeRefreshToken();

        #endregion

        #region User

        Task<string> GetEmailByUserId(string userId);

        #endregion

        #region Register

        Task<Response<RegisterResponseViewModel>> RegisterAsync(UserRegistrationViewModel userRegistrationModel);

        Task<Response<RegisterConfirmationViewModel>> GenerateEmailConfirmationTokenAsync(RegisterConfirmationViewModel registerConfirmationViewModel);

        #endregion

        #region ForgotPassword

        Task<Response<ForgotPasswordViewModel>> ForgotPasswordAsync(ForgotPasswordViewModel forgotPasswordViewModel);

        Task<Response<bool>> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel);

        #endregion

        #region ChangePassword

        Task<Response<ChangePasswordViewModel>> ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel);

        Task<Response<SetPasswordViewModel>> SetPasswordAsync(SetPasswordViewModel setPasswordViewModel);

        Task<Response<bool>> HasPasswordAsync(string userId);

        #endregion

        #region PersonalData

        Task<Response<DeletePersonalDataViewModel>> DeletePersonalDataAsync(DeletePersonalDataViewModel deletePersonalDataViewModel);

        Task<Response<Dictionary<string, string>>> DownloadPersonalDataAsync(DownloadPersonalDataViewModel downloadPersonalDataViewModel);

        #endregion

        #region Disable2fa

        Task<Response<bool>> GetTwoFactorEnabledAsync(string userId);

        Task<Response<bool>> SetTwoFactorEnabledAsync(string userId);



        #endregion

        #region ChangeEmail

        Task<Response<bool>> IsEmailConfirmedAsync(string userId);

        Task<Response<ChangeEmailViewModel>> ChangeEmailAsync(ChangeEmailViewModel changeEmailViewModel);

        Task<Response<SendEmailConfirmationViewModel>> SendVerificationEmailAsync(SendEmailConfirmationViewModel sendEmailConfirmationViewModel);

        Task<Response<ConfirmEmailViewModel>> ConfirmEmailAsync(ConfirmEmailViewModel confirmEmailViewModel);

        Task<Response<ConfirmEmailChangeViewModel>> ConfirmEmailChangeAsync(ConfirmEmailChangeViewModel confirmEmailChangeViewModel);

        #endregion

        #region EnableAuthenticator

        Task<Response<EnableAuthenticatorViewModel>> LoadSharedKeyAndQrCodeUriAsync(string userId);

        Task<Response<EnableAuthenticatorViewModel>> VerifyAsync(EnableAuthenticatorViewModel enableAuthenticatorViewModel);

        #endregion

        #region TwoFactorAuthentication

        Task<Response<TwoFactorAuthenticationViewModel>> GetTwoFactorAuthenticationAsync(string userId);

        Task<Response<bool>> ForgetTwoFactorClientAsync(string userId);
        #endregion

        #region ResetAuthenticator

        Task<Response<bool>> ResetAuthenticatorAsync(string userId);

        #endregion

        #region GenerateRecoveryCodes

        Task<Response<GenerateRecoveryCodesViewModel>> GenerateRecoveryCodesAsync(string userId);

        #endregion

        #region ExternalLogins

        Task<Response<ExternalLoginsViewModel>> GetExternalLoginsAsync(string userId);

        Task<Response<bool>> RemoveExternalLoginsAsync(RemoveExternalLoginViewModel removeExternalLoginViewModel);

        Task<Response<ChallengeResult>> LinkExternalLoginsAsync(LinkExternalLoginViewModel linkExternalLoginViewModel);

        Task<Response<bool>> LinkLoginCallbackAsync(string userId);

        #endregion

        #region UserProfile

        Task<Response<ApplicationUser>> GetUserProfileAsync(string userId = null);

        Task<Response<bool>> UpdateProfileAsync(ApplicationUser applicationUser);

        #endregion
    }
}
