using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models;
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
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;
        private readonly ISharedIdentityService _sharedIdentityService;

        public IdentityService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings,
           IOptions<ServiceApiSettings> serviceApiSettings, ISharedIdentityService sharedIdentityService)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
            _sharedIdentityService = sharedIdentityService;
        }

        #region Token

        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }
            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            RefreshTokenRequest refreshTokenRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                RefreshToken = refreshToken,
                Address = disco.TokenEndpoint
            };

            //if (refreshToken != null)
            //{

            //}
            //else
            //{
            //    refreshTokenRequest = new()
            //    {
            //        ClientId = _clientSettings.WebClient.ClientId,
            //        ClientSecret = _clientSettings.WebClient.ClientSecret,
            //        RefreshToken = refreshToken,
            //        Address = disco.TokenEndpoint
            //    };
            //}



            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);
            if (token.IsError)
            {
                return null;
            }

            var authenticationTokens = new List<AuthenticationToken>()
            {
                new AuthenticationToken{Name =OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{Name =OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
                new AuthenticationToken{Name =OpenIdConnectParameterNames.ExpiresIn,
                    Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)},
            };


            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();

            var properties = authenticationResult.Properties;

            properties.StoreTokens(authenticationTokens);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                authenticationResult.Principal, properties);

            return token;


        }

        public async Task RevokeRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(
                OpenIdConnectParameterNames.RefreshToken);

            TokenRevocationRequest tokenRevocationRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                Address = disco.RevocationEndpoint,
                Token = refreshToken,
                TokenTypeHint = "refresh_token"
            };

            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);
        }

        #endregion

        #region SignIn

        public async Task<Response<bool>> SignIn(SigninInput signInInput)
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                UserName = signInInput.Email,
                Password = signInInput.Password,
                Address = disco.TokenEndpoint
            };


            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

            if (token.IsError)
            {
                var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();
                var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Response<bool>.Fail(errorDto.Errors, 400);
            }

            var userInfoRequest = new UserInfoRequest
            {
                Token = token.AccessToken,
                Address = disco.UserInfoEndpoint
            };

            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);
            if (userInfo.IsError)
            {
                throw userInfo.Exception;
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                "name", "role");

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            {
                new AuthenticationToken{Name =OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{Name =OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
                new AuthenticationToken{Name =OpenIdConnectParameterNames.ExpiresIn,
                    Value=DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)},
            });

            authenticationProperties.IsPersistent = signInInput.IsRemember;
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                authenticationProperties);

            return Response<bool>.Success(200);
        }


        public async Task<Response<SigninInput>> LoginAsync(SigninInput signInInput)
        {
            if (signInInput == null)
                return Response<SigninInput>.Fail("Sign in parameters has not empty", 404);

            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/login", signInInput);
            if (!response.IsSuccessStatusCode)
                return Response<SigninInput>.Fail("An error occurred while logging", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<SigninInput>>();
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
                return Response<SigninInput>.Fail(result.Erros, 404);

            return Response<SigninInput>.Success(result.Data, 200);
        }

        #endregion

        #region User

        public async Task<string> GetEmailByUserId(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/GetEmailByUserId/{userId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var email = await response.Content.ReadFromJsonAsync<Response<string>>();
            if (email == null)
                return null;
            return email.Data;
        }

        #endregion

        #region Register

        public async Task<Response<RegisterResponseViewModel>> RegisterAsync(UserRegistrationViewModel userRegistrationModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/Register", userRegistrationModel);
            if (!response.IsSuccessStatusCode)
                return Response<RegisterResponseViewModel>.Fail("User couldnt created.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<RegisterResponseViewModel>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<RegisterResponseViewModel>.Fail(result.Erros, 404);

            return Response<RegisterResponseViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<RegisterConfirmationViewModel>> GenerateEmailConfirmationTokenAsync(RegisterConfirmationViewModel registerConfirmationViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/generate-email-confirmation-token", registerConfirmationViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<RegisterConfirmationViewModel>.Fail("User email confirmation token couldnt generated error.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<RegisterConfirmationViewModel>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<RegisterConfirmationViewModel>.Fail(result.Erros, 404);

            return Response<RegisterConfirmationViewModel>.Success(result.Data, 200);
        }

        #endregion

        #region ForgotPassword

        public async Task<Response<ForgotPasswordViewModel>> ForgotPasswordAsync(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/SendResetPassword", forgotPasswordViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<ForgotPasswordViewModel>.Fail("User password couldnt reset.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<ForgotPasswordViewModel>>();
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
                return Response<ForgotPasswordViewModel>.Fail(result.Erros, 404);
            return Response<ForgotPasswordViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<bool>> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/ResetPassword", resetPasswordViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("User password couldnt reset.", 404);
            return Response<bool>.Success(true, 200);
        }

        #endregion

        #region ChangePassword

        public async Task<Response<bool>> HasPasswordAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/HasPasswordAsync/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("User password couldnt change.", 404);

            var hasPassword = await response.Content.ReadFromJsonAsync<Response<bool>>();
            if (!hasPassword.Data)
                return Response<bool>.Fail("User have not password", 404);
            return Response<bool>.Success(true, 200);
        }

        public async Task<Response<ChangePasswordViewModel>> ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/ChangePasswordAsync", changePasswordViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<ChangePasswordViewModel>.Fail("User password couldnt change.", 404);
            var result = await response.Content.ReadFromJsonAsync<Response<ChangePasswordViewModel>>();
            if (result.Data == null)
                return Response<ChangePasswordViewModel>.Fail("User password couldnt change", 404);

            return Response<ChangePasswordViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<SetPasswordViewModel>> SetPasswordAsync(SetPasswordViewModel setPasswordViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/SetPasswordAsync", setPasswordViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<SetPasswordViewModel>.Fail("User password couldnt set.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<SetPasswordViewModel>>();
            if (result.Data == null)
                return Response<SetPasswordViewModel>.Fail("User password couldnt set", 404);
            return Response<SetPasswordViewModel>.Success(result.Data, 200);
        }

        #endregion

        #region PersonalData

        public async Task<Response<DeletePersonalDataViewModel>> DeletePersonalDataAsync(DeletePersonalDataViewModel deletePersonalDataViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/DeletePersonalDataAsync", deletePersonalDataViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<DeletePersonalDataViewModel>.Fail("User's data couldnt delete", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<DeletePersonalDataViewModel>>();
            if (result.Data == null)
                return Response<DeletePersonalDataViewModel>.Fail("User's data couldnt delete", 404);
            return Response<DeletePersonalDataViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<Dictionary<string, string>>> DownloadPersonalDataAsync(DownloadPersonalDataViewModel downloadPersonalDataViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/DownloadPersonalDataAsync", downloadPersonalDataViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<Dictionary<string, string>>.Fail("User's data couldnt download", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<Dictionary<string, string>>>();
            if (result.Data == null || !result.Data.Any())
                return Response<Dictionary<string, string>>.Fail("User's data couldnt download", 404);
            return Response<Dictionary<string, string>>.Success(result.Data, 200);
        }

        #endregion

        #region Disable2Fa

        public async Task<Response<bool>> GetTwoFactorEnabledAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/GetTwoFactorEnabledAsync/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("Get two factor enabled error", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            return Response<bool>.Success(result.Data, 200);
        }

        public async Task<Response<bool>> SetTwoFactorEnabledAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/SetTwoFactorEnabledAsync/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("Set two factor enabled error", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            return Response<bool>.Success(result.Data, 200);
        }

        #endregion

        #region ChangeEmail

        public async Task<Response<bool>> IsEmailConfirmedAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/IsEmailConfirmed/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("User's email not confirmed error", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            return Response<bool>.Success(result.Data, 200);
        }

        public async Task<Response<ChangeEmailViewModel>> ChangeEmailAsync(ChangeEmailViewModel changeEmailViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/ChangeEmail", changeEmailViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<ChangeEmailViewModel>.Fail("User's chnage email error", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<ChangeEmailViewModel>>();
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
                return Response<ChangeEmailViewModel>.Fail(result.Erros, 404);
            return Response<ChangeEmailViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<SendEmailConfirmationViewModel>> SendVerificationEmailAsync(SendEmailConfirmationViewModel sendEmailConfirmationViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/SendVerificationEmail", sendEmailConfirmationViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<SendEmailConfirmationViewModel>.Fail("User send email verification error", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<SendEmailConfirmationViewModel>>();
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
                return Response<SendEmailConfirmationViewModel>.Fail(result.Erros, 404);
            return Response<SendEmailConfirmationViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<ConfirmEmailViewModel>> ConfirmEmailAsync(ConfirmEmailViewModel confirmEmailViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/ConfirmEmail", confirmEmailViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<ConfirmEmailViewModel>.Fail("User confirm email error", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<ConfirmEmailViewModel>>();
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
                return Response<ConfirmEmailViewModel>.Fail(result.Erros, 404);
            return Response<ConfirmEmailViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<ConfirmEmailChangeViewModel>> ConfirmEmailChangeAsync(ConfirmEmailChangeViewModel confirmEmailChangeViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/ConfirmEmailChange", confirmEmailChangeViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<ConfirmEmailChangeViewModel>.Fail("User confirm email change error", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<ConfirmEmailChangeViewModel>>();
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
                return Response<ConfirmEmailChangeViewModel>.Fail(result.Erros, 404);
            return Response<ConfirmEmailChangeViewModel>.Success(result.Data, 200);
        }

        #endregion

        #region EnableAuthenticator

        public async Task<Response<EnableAuthenticatorViewModel>> LoadSharedKeyAndQrCodeUriAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/LoadSharedKeyAndQrCodeUriAsync/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<EnableAuthenticatorViewModel>.Fail("User's enable authenticator key found error", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<EnableAuthenticatorViewModel>>();
            return Response<EnableAuthenticatorViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<EnableAuthenticatorViewModel>> VerifyAsync(EnableAuthenticatorViewModel enableAuthenticatorViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/VerifyAsync", enableAuthenticatorViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<EnableAuthenticatorViewModel>.Fail("User's verify enable authenticator error", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<EnableAuthenticatorViewModel>>();
            return Response<EnableAuthenticatorViewModel>.Success(result.Data, 200);
        }


        #endregion

        #region TwoFactorAuthentication

        public async Task<Response<TwoFactorAuthenticationViewModel>> GetTwoFactorAuthenticationAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/GetTwoFactorAuthentication/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<TwoFactorAuthenticationViewModel>.Fail("An error occurred while user's two factor authentication getting", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<TwoFactorAuthenticationViewModel>>();
            return Response<TwoFactorAuthenticationViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<bool>> ForgetTwoFactorClientAsync(string userId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/ForgetTwoFactorClient", userId);
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("An error occurred while user's two factor authentication getting", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            return Response<bool>.Success(result.Data, 200);
        }

        #endregion

        #region ResetAuthenticator

        public async Task<Response<bool>> ResetAuthenticatorAsync(string userId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/ResetAuthenticator", userId);
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("An error occurred while user's two factor authentication getting", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            return Response<bool>.Success(result.Data, 200);
        }

        #endregion

        #region GenerateRecoveryCodes

        public async Task<Response<GenerateRecoveryCodesViewModel>> GenerateRecoveryCodesAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/GenerateRecoveryCodes/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<GenerateRecoveryCodesViewModel>.Fail("An error occurred while user's recovery code generating", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<GenerateRecoveryCodesViewModel>>();
            return Response<GenerateRecoveryCodesViewModel>.Success(result.Data, 200);
        }

        #endregion

        #region ExternalLogins

        public async Task<Response<ExternalLoginsViewModel>> GetExternalLoginsAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/GetExternalLogins/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<ExternalLoginsViewModel>.Fail("An error occurred while getting user's external logins", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<ExternalLoginsViewModel>>();
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
                return Response<ExternalLoginsViewModel>.Fail(result.Erros, 404);

            return Response<ExternalLoginsViewModel>.Success(result.Data, 200);
        }
        public async Task<Response<bool>> RemoveExternalLoginsAsync(RemoveExternalLoginViewModel removeExternalLoginViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/RemoveExternalLogins", removeExternalLoginViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("An error occurred while removing user's external logins", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            return Response<bool>.Success(result.Data, 200);
        }

        public async Task<Response<ChallengeResult>> LinkExternalLoginsAsync(LinkExternalLoginViewModel linkExternalLoginViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/LinkExternalLogins", linkExternalLoginViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<ChallengeResult>.Fail("An error occurred while logging with external logins", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<ChallengeResult>>();
            return Response<ChallengeResult>.Success(result.Data, 200);
        }

        public async Task<Response<bool>> LinkLoginCallbackAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/LinkLoginCallback/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("An error occurred while logging with external logins", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            return Response<bool>.Success(result.Data, 200);
        }

        #endregion

        #region UserProfile

        public async Task<Response<ApplicationUser>> GetUserProfileAsync(string userId = null)
        {
            if (userId == null)
                userId = _sharedIdentityService.GetUserId;
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/GetUserProfile/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<ApplicationUser>.Fail("An error occurred while getting user's profile information", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<ApplicationUser>>();
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
                return Response<ApplicationUser>.Fail(result.Erros, 404);

            return Response<ApplicationUser>.Success(result.Data, 200);
        }

        public async Task<Response<bool>> UpdateProfileAsync(ApplicationUser applicationUser)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Account/UpdateProfile", applicationUser);
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("An error occurred while updating user's profile", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            if (!result.Data || (result.Erros != null && result.Erros.Any()))
                return Response<bool>.Fail(result.Erros, 404);

            return Response<bool>.Success(result.Data, 200);
        }

        #endregion
    }
}
