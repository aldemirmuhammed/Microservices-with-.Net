using User.Management.API.Models;
using FreeCourse.Shared.Dtos;
using User.Management.API.Dtos.ChangePassword;
using User.Management.API.Dtos.ResetPassword;
using User.Management.API.Dtos.PersonalData;
using User.Management.API.Dtos.ChangeEmail;
using User.Management.API.Dtos.EnableAuthenticator;
using User.Management.API.Dtos.TwoFactorAuthentication;
using Microsoft.AspNetCore.Identity;
using User.Management.API.Dtos.GenerateRecoveryCodes;
using User.Management.API.Dtos.ExternalLogins;
using Microsoft.AspNetCore.Mvc;
using User.Management.API.Dtos.ConfirmEmail;
using User.Management.API.Dtos.ConfirmEmailChange;
using User.Management.API.Dtos.SendEmailConfirmation;
using User.Management.API.Dtos.Register;
using User.Management.API.Dtos.Login;
using User.Management.API.Dtos.ForgotPassword;

namespace User.Management.API.Services.Interfaces
{
    public interface IUserManagement
    {

        /// <summary>
        /// Brief description of what the method does.
        /// </summary>
        /// <param name="registerUser">Description of the parameter.</param>
        /// <returns>Description of the return value.</returns>

        Task<Response<RegisterResponseDto>> CreateUserWithTokenAsync(RegisterDto registerUser);
        Task<Response<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user);
        Task<Response<LoginOtpResponseDto>> GetOtpByLoginAsync(LoginDto loginModel);
        Task<Response<LoginResponseDto>> GetJwtTokenAsync(ApplicationUser user);
        Task<Response<LoginResponseDto>> LoginUserWithJWTokenAsync(string otp, string userName);
        Task<Response<LoginResponseDto>> RenewAccessTokenAsync(LoginResponseDto tokens);
        Task<ApplicationUser?> FindByIdAsync(string userId);

        #region Login

        Task<Response<LoginDto>> LoginAsync(LoginDto loginDto);

        Task<Response<LoginResponseDto>> RefreshTokenAsync(LoginResponseDto loginResponseDto);

        Task<Response<LoginResponseDto>> LoginWithOTPAsync(string code, string userName);

        #endregion

        #region Register

        Task<Response<RegisterResponseDto>> RegisterAsync(RegisterDto registerUser);

        Task<Response<RegisterConfirmationDto>> GenerateEmailConfirmationTokenAsync(RegisterConfirmationDto registerConfirmationDto);

        #endregion

        #region ResetPassword

        Task<Response<ForgotPasswordDto>> SendResetPasswordAsync(ForgotPasswordDto forgotPassword);

        Task<Response<ResetPasswordDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        #endregion

        #region ChangePassword

        Task<Response<bool>> HasPasswordAsync(string userId);

        Task<Response<ChangePasswordDto>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);

        Task<Response<SetPasswordDto>> SetPasswordAsync(SetPasswordDto setpasswordDto);

        #endregion

        #region PersonalData

        Task<Response<DeletePersonalDataDto>> DeletePersonalDataAsync(DeletePersonalDataDto deletePersonalDataDto);

        Task<Response<Dictionary<string, string>>> DownloadPersonalDataAsync(DownloadPersonalDataDto DownloadPersonalDataDto);
        #endregion

        #region Disable2Fa

        Task<Response<bool>> GetTwoFactorEnabledAsync(string userId);

        Task<Response<bool>> SetTwoFactorEnabledAsync(string userId);

        #endregion

        #region ChangeEmail

        Task<Response<bool>> IsEmailConfirmedAsync(string userId);

        Task<Response<ChangeEmailDto>> ChangeEmailAsync(ChangeEmailDto changeEmailDto);

        Task<Response<ConfirmEmailChangeDto>> ConfirmEmailChangeAsync(ConfirmEmailChangeDto confirmEmailChangeDto);

        Task<Response<SendEmailConfirmationDto>> SendVerificationEmailAsync(SendEmailConfirmationDto sendEmailConfirmationDto);

        Task<Response<ConfirmEmailDto>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);

        #endregion

        #region EnableAuthenticator

        Task<Response<EnableAuthenticatorDto>> EnableAuthenticatorLoadSharedKeyAndQrCodeUriAsync(string userId);

        Task<Response<EnableAuthenticatorDto>> VerifyAsync(EnableAuthenticatorDto enableAuthenticatorDto);
        #endregion

        #region TwoFactorAuthentication

        Task<Response<TwoFactorAuthenticationDto>> GetTwoFactorAuthenticationAsync(string userId);

        Task<Response<bool>> ForgetTwoFactorClientAsync(string userId);


        #endregion

        #region ResetAuthenticator


        Task<Response<bool>> ResetAuthenticatorAsync(string userId);


        #endregion

        #region GenerateRecoveryCodes

        Task<Response<GenerateRecoveryCodesDto>> GenerateRecoveryCodesAsync(string userId);

        #endregion

        #region ExternalLogins

        Task<Response<ExternalLoginsDto>> GetExternalLoginsAsync(string userId);

        Task<Response<bool>> RemoveExternalLoginsAsync(RemoveExternalLoginDto removeExternalLoginDto);

        Task<Response<ChallengeResult>> LinkExternalLoginsAsync(LinkExternalLoginDto linkExternalLoginDto);

        Task<Response<bool>> LinkLoginCallbackAsync(string userId);

        #endregion

        #region UserProfile

        Task<Response<ApplicationUser>> GetUserProfileAsync(string userId);

        Task<Response<bool>> UpdateProfileAsync(ApplicationUser applicationUser);

        #endregion

    }
}
