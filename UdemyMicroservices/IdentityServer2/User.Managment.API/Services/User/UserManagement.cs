using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using User.Management.API.Constants;
using User.Management.API.Dtos.Authentication.User;
using User.Management.API.Dtos.ChangeEmail;
using User.Management.API.Dtos.ChangePassword;
using User.Management.API.Dtos.ConfirmEmail;
using User.Management.API.Dtos.ConfirmEmailChange;
using User.Management.API.Dtos.EnableAuthenticator;
using User.Management.API.Dtos.ExternalLogins;
using User.Management.API.Dtos.ForgotPassword;
using User.Management.API.Dtos.GenerateRecoveryCodes;
using User.Management.API.Dtos.Login;
using User.Management.API.Dtos.PersonalData;
using User.Management.API.Dtos.Register;
using User.Management.API.Dtos.ResetPassword;
using User.Management.API.Dtos.SendEmailConfirmation;
using User.Management.API.Dtos.TwoFactorAuthentication;
using User.Management.API.Models;
using User.Management.API.Services.Interfaces;

namespace User.Management.API.Services.User
{
    public class UserManagement : IUserManagement
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly UrlEncoder _urlEncoder;


        public UserManagement(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _urlEncoder = urlEncoder;
        }

        public async Task<Response<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user)
        {
            if (roles == null || roles.Count == 0)
                return Response<List<string>>.Fail("Roles list empty error", 404);

            var assignedRole = new List<string>();
            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    if (!await _userManager.IsInRoleAsync(user, role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        assignedRole.Add(role);
                    }
                }
            }

            var getUserRoles = await _userManager.GetRolesAsync(user);

            return Response<List<string>>.Success(getUserRoles.ToList(), 200);

        }

        public async Task<Response<RegisterResponseDto>> CreateUserWithTokenAsync(RegisterDto registerUser)
        {
            if (registerUser == null || registerUser.Email == null || registerUser.Password == null)
                return Response<RegisterResponseDto>.Fail("User has not null error", 404);

            //Check User Exist 
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
                return Response<RegisterResponseDto>.Fail("User already exist error", 404);

            ApplicationUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username,
                PhoneNumber = registerUser.Phone,
                TwoFactorEnabled = true

            };
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                return Response<RegisterResponseDto>.Success(new RegisterResponseDto { Token = token, User = user }, 201);
            }
            return Response<RegisterResponseDto>.Fail("User create error", 404);

        }

        public async Task<Response<LoginOtpResponseDto>> GetOtpByLoginAsync(LoginDto loginModel)
        {
            if (loginModel == null || loginModel.Email == null || loginModel.Password == null)
                return Response<LoginOtpResponseDto>.Fail("User parameter has not empty error", 404);
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, true);
                if (user.TwoFactorEnabled)
                {
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    return Response<LoginOtpResponseDto>.Success(new LoginOtpResponseDto
                    {
                        User = user,
                        Token = token,
                        IsTwoFactorEnable = user.TwoFactorEnabled
                    }, 200);
                }
                else
                {
                    return Response<LoginOtpResponseDto>.Success(new LoginOtpResponseDto
                    {
                        User = user,
                        Token = string.Empty,
                        IsTwoFactorEnable = user.TwoFactorEnabled
                    }, 200);
                }
            }
            else
                return Response<LoginOtpResponseDto>.Fail("User dosnt exist error", 404);
        }

        public async Task<Response<LoginResponseDto>> GetJwtTokenAsync(ApplicationUser user)
        {
            if (user == null || user.UserName == null)
                return Response<LoginResponseDto>.Fail("User parameter hasnt empty error", 404);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtToken = GetToken(authClaims); //access token
            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidity"], out int refreshTokenValidity);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenValidity);

            await _userManager.UpdateAsync(user);

            return Response<LoginResponseDto>.Success(new LoginResponseDto
            {
                AccessToken = new TokenType()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    ExpiryTokenDate = jwtToken.ValidTo
                },
                RefreshToken = new TokenType()
                {
                    Token = user.RefreshToken,
                    ExpiryTokenDate = (DateTime)user.RefreshTokenExpiry
                }
            }, 200);
        }

        public async Task<Response<LoginResponseDto>> LoginUserWithJWTokenAsync(string otp, string userName)
        {
            if (otp == null || userName == null)
                return Response<LoginResponseDto>.Fail("User parameter hasnt empty error", 404);
            var user = await _userManager.FindByNameAsync(userName);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", otp, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
                    return await GetJwtTokenAsync(user);
            }

            return Response<LoginResponseDto>.Fail("Invalid otp error", 404);
        }

        public async Task<Response<LoginResponseDto>> RenewAccessTokenAsync(LoginResponseDto tokens)
        {
            var accessToken = tokens.AccessToken;
            var refreshToken = tokens.RefreshToken;
            var principal = GetClaimsPrincipal(accessToken.Token);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            if (refreshToken.Token != user.RefreshToken && refreshToken.ExpiryTokenDate <= DateTime.Now)
                return Response<LoginResponseDto>.Fail("Token invalid or expired error", 404);

            var response = await GetJwtTokenAsync(user);
            return response;
        }

        public async Task<ApplicationUser?> FindByIdAsync(string userId)
        {
            try
            {
                return await _userManager.FindByIdAsync(userId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region Login

        public async Task<Response<LoginDto>> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null)
                return Response<LoginDto>.Fail("User parameter hasnt empty error", 404);

            if (loginDto.Password == null)
                return Response<LoginDto>.Fail("User password hasnt empty error", 404);

            var loginOtpResponse = await GetOtpByLoginAsync(loginDto);
            if (loginOtpResponse.Data != null)
            {
                var user = loginOtpResponse.Data.User;
                if (user.TwoFactorEnabled)
                {
                    var token = loginOtpResponse.Data.Token;
                    //var message = new EmailMessage(new string[] { user.Email! }, "OTP Confrimation", token, _textFormat: TextFormat.Text);
                    //await _emailService.SendEmailAsync(message);
                    return Response<LoginDto>.Success(loginDto, 200);
                }
                if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    var serviceResponse = await GetJwtTokenAsync(user);
                    return Response<LoginDto>.Success(loginDto, 200);
                }
            }
            return Response<LoginDto>.Fail("User login error", 404);
        }

        public async Task<Response<LoginResponseDto>> LoginWithOTPAsync(string code, string userName)
        {
            var jwt = await LoginUserWithJWTokenAsync(code, userName);
            if (jwt.Data != null)
            {
                return Response<LoginResponseDto>.Success(jwt.Data, 200);
            }

            return Response<LoginResponseDto>.Fail("Invalid OTP code error", 404);
        }

        public async Task<Response<LoginResponseDto>> RefreshTokenAsync(LoginResponseDto loginResponseDto)
        {
            var jwt = await RenewAccessTokenAsync(loginResponseDto);
            if (jwt.Data != null)
                return Response<LoginResponseDto>.Success(jwt.Data, 200);
            return Response<LoginResponseDto>.Fail("Invalid refresh code error", 404);
        }

        #endregion

        #region Register

        public async Task<Response<RegisterResponseDto>> RegisterAsync(RegisterDto registerUser)
        {
            try
            {
                if (registerUser == null)
                    return Response<RegisterResponseDto>.Fail("User parameter hasnt empty error", 404);

                var tokenResponse = await CreateUserWithTokenAsync(registerUser);
                if (tokenResponse.Data != null && tokenResponse.Data.Token != null)
                {
                    if (registerUser.Roles != null && registerUser.Roles.Count != 0)
                        await AssignRoleToUserAsync(registerUser.Roles, tokenResponse.Data.User);

                    registerUser.RequireConfirmedAccount = true;
                    tokenResponse.Data.RequireConfirmedAccount = true;
                    return Response<RegisterResponseDto>.Success(tokenResponse.Data, 200);
                }
                if (tokenResponse.Erros != null && tokenResponse.Erros.Any())
                    return Response<RegisterResponseDto>.Fail(tokenResponse.Erros, 404);
                return Response<RegisterResponseDto>.Fail("User couldnt register error", 404);
            }
            catch (Exception ex)
            {
                return Response<RegisterResponseDto>.Fail(ex.Message, 404);

            }
        }

        public async Task<Response<RegisterConfirmationDto>> GenerateEmailConfirmationTokenAsync(RegisterConfirmationDto registerConfirmationDto)
        {
            try
            {
                if (registerConfirmationDto.Email == null)
                    return Response<RegisterConfirmationDto>.Fail("User's mail couldn't found error.", 404);

                var user = await _userManager.FindByEmailAsync(registerConfirmationDto.Email);
                if (user == null)
                    return Response<RegisterConfirmationDto>.Fail("User's mail couldn't found error", 404);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                registerConfirmationDto.Code = EncodeString(code);
                registerConfirmationDto.UserId = EncodeString(user.Id);

                return Response<RegisterConfirmationDto>.Success(registerConfirmationDto, 200);
            }
            catch (Exception ex)
            {
                return Response<RegisterConfirmationDto>.Fail(ex.Message, 404);
            }
        }

        #endregion

        #region ResetPassword

        public async Task<Response<ForgotPasswordDto>> SendResetPasswordAsync(ForgotPasswordDto forgotPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    return Response<ForgotPasswordDto>.Fail("User's mail couldn't found.", 404);


                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (code == null)
                    return Response<ForgotPasswordDto>.Fail("User's code  couldn't generated.", 404);

                forgotPassword.Code = EncodeString(code);
                forgotPassword.UserId = EncodeString(user.Id);
                return Response<ForgotPasswordDto>.Success(forgotPassword, 200);
            }
            catch (Exception ex)
            {
                return Response<ForgotPasswordDto>.Fail(ex.Message, 404);
            }
        }

        public async Task<Response<ResetPasswordDto>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (resetPasswordDto == null || resetPasswordDto.Email == null || resetPasswordDto.UserId == null || resetPasswordDto.Code == null)
                    return Response<ResetPasswordDto>.Fail("User's mail couldn't found", 404);

                resetPasswordDto.UserId = DecodeString(resetPasswordDto.UserId);
                resetPasswordDto.Code = DecodeString(resetPasswordDto.Code);

                var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
                if (user == null)
                    return Response<ResetPasswordDto>.Fail("User's mail couldn't found", 404);


                var code = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Code, resetPasswordDto.Password);
                if (code == null)
                    return Response<ResetPasswordDto>.Fail("User's code empty", 404);
                return Response<ResetPasswordDto>.Success(resetPasswordDto, 200);
            }
            catch (Exception ex)
            {
                return Response<ResetPasswordDto>.Fail(ex.Message, 404);
            }
        }


        #endregion  

        #region ChangePassword

        // Change password get method
        public async Task<Response<bool>> HasPasswordAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                    return Response<bool>.Fail("User has not password", 404);

                return Response<bool>.Success(true, 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(ex.Message, 404);
            }
        }

        // Change password post method
        public async Task<Response<ChangePasswordDto>> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (changePasswordDto == null || changePasswordDto.UserId == null)
                    return Response<ChangePasswordDto>.Fail("Error Change password parameters havent empty.", 404);

                var user = await FindByIdAsync(changePasswordDto.UserId);
                if (user == null)
                    return Response<ChangePasswordDto>.Fail("Error User couldnt found", 404);

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
                if (!changePasswordResult.Succeeded)
                    return Response<ChangePasswordDto>.Fail("Error Operation couldnt complete", 404);

                changePasswordDto.StatusMessage = "Success Password changing complete successfully";
                changePasswordDto.ConfirmPassword = string.Empty;
                changePasswordDto.NewPassword = string.Empty;
                changePasswordDto.OldPassword = string.Empty;
                return Response<ChangePasswordDto>.Success(changePasswordDto, 200);
            }
            catch (Exception ex)
            {
                return Response<ChangePasswordDto>.Fail(ex.Message, 404);
            }
        }


        public async Task<Response<SetPasswordDto>> SetPasswordAsync(SetPasswordDto setpasswordDto)
        {
            try
            {
                if (setpasswordDto == null || setpasswordDto.UserId == null)
                    return Response<SetPasswordDto>.Fail("Setting password parameters havent empty.", 404);

                var user = await FindByIdAsync(setpasswordDto.UserId);
                if (user == null)
                    return Response<SetPasswordDto>.Fail("Unexpected error when trying to found user", 404);

                var addPasswordResult = await _userManager.AddPasswordAsync(user, setpasswordDto.NewPassword);
                if (!addPasswordResult.Succeeded)
                    return Response<SetPasswordDto>.Fail("Operation couldnt complete", 404);

                setpasswordDto.StatusMessage = "Password setting complete successfully";
                setpasswordDto.NewPassword = string.Empty;
                setpasswordDto.ConfirmPassword = string.Empty;
                return Response<SetPasswordDto>.Success(setpasswordDto, 200);
            }
            catch (Exception ex)
            {
                return Response<SetPasswordDto>.Fail(ex.Message, 404);
            }
        }

        #endregion

        #region PersonalData

        public async Task<Response<DeletePersonalDataDto>> DeletePersonalDataAsync(DeletePersonalDataDto deletePersonalDataDto)
        {
            try
            {
                if (deletePersonalDataDto == null || deletePersonalDataDto.Password == null || deletePersonalDataDto.UserId == null)
                    return Response<DeletePersonalDataDto>.Fail("User data parameter has not empty", 404);


                var user = await FindByIdAsync(deletePersonalDataDto.UserId);
                if (user == null)
                    return Response<DeletePersonalDataDto>.Fail("Unexpected error when trying to found user", 404);


                var checkPassword = await _userManager.CheckPasswordAsync(user, deletePersonalDataDto.Password);
                if (!checkPassword)
                    return Response<DeletePersonalDataDto>.Fail("User's password couldnt checked", 404);

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return Response<DeletePersonalDataDto>.Fail("User's data couldnt delete", 404);

                await _signInManager.SignOutAsync();


                return Response<DeletePersonalDataDto>.Success(deletePersonalDataDto, 200);
            }
            catch (Exception ex)
            {
                return Response<DeletePersonalDataDto>.Fail(ex.Message, 404);
            }
        }


        public async Task<Response<Dictionary<string, string>>> DownloadPersonalDataAsync(DownloadPersonalDataDto downloadPersonalDataDto)
        {
            try
            {
                if (downloadPersonalDataDto == null || downloadPersonalDataDto.UserId == null)
                    return Response<Dictionary<string, string>>.Fail("User data parameter has not empty", 404);


                var user = await FindByIdAsync(downloadPersonalDataDto.UserId);
                if (user == null)
                    return Response<Dictionary<string, string>>.Fail("Unexpected error when trying to found user", 404);

                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps)
                {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var l in logins)
                {
                    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                }
                return Response<Dictionary<string, string>>.Success(personalData, 200);
            }
            catch (Exception ex)
            {
                return Response<Dictionary<string, string>>.Fail(ex.Message, 404);
            }

        }

        #endregion

        #region Disable2Fa

        public async Task<Response<bool>> GetTwoFactorEnabledAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);

                var isEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                if (!isEnabled)
                    return Response<bool>.Fail($"Cannot disable 2FA for user with ID '{user.UserName}' as it's not currently enabled.", 404);

                return Response<bool>.Success(isEnabled, 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(ex.Message, 404);
            }
        }

        public async Task<Response<bool>> SetTwoFactorEnabledAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);

                var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                if (!disable2faResult.Succeeded)
                    return Response<bool>.Fail($"Unexpected error occurred disabling 2FA for user with ID '{user.UserName}'", 404);
                return Response<bool>.Success(disable2faResult.Succeeded, 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(ex.Message, 404);
            }
        }

        #endregion

        #region ChangeEmail

        public async Task<Response<bool>> IsEmailConfirmedAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);

                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (!isEmailConfirmed)
                    return Response<bool>.Fail($"User email not confirmed", 404);
                return Response<bool>.Success(isEmailConfirmed, 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(ex.Message, 404);
            }
        }

        public async Task<Response<ChangeEmailDto>> ChangeEmailAsync(ChangeEmailDto changeEmailDto)
        {
            try
            {
                if (changeEmailDto == null || changeEmailDto.UserId == null)
                    return Response<ChangeEmailDto>.Fail("Unexpected error when trying to found user", 404);

                var user = await FindByIdAsync(changeEmailDto.UserId);
                if (user == null)
                    return Response<ChangeEmailDto>.Fail("Unexpected error when trying to found user", 404);

                var email = await _userManager.GetEmailAsync(user);
                if (changeEmailDto.NewEmail != email)
                {
                    var userId = changeEmailDto.UserId;

                    var code = await _userManager.GenerateChangeEmailTokenAsync(user, changeEmailDto.NewEmail);

                    changeEmailDto.Token = EncodeString(code);
                    changeEmailDto.UserId = EncodeString(userId);
                    changeEmailDto.NewEmail = EncodeString(changeEmailDto.NewEmail);

                    return Response<ChangeEmailDto>.Success(changeEmailDto, 200);
                }
                return Response<ChangeEmailDto>.Fail("User's new email match current email", 404);
            }
            catch (Exception ex)
            {
                return Response<ChangeEmailDto>.Fail(ex.Message, 404);
            }

        }

        public async Task<Response<ConfirmEmailChangeDto>> ConfirmEmailChangeAsync(ConfirmEmailChangeDto confirmEmailChangeDto)
        {
            try
            {
                if (confirmEmailChangeDto.UserId == null)
                    return Response<ConfirmEmailChangeDto>.Fail("Unexpected error when trying to found user", 404);

                confirmEmailChangeDto.UserId = DecodeString(confirmEmailChangeDto.UserId);
                confirmEmailChangeDto.Code = DecodeString(confirmEmailChangeDto.Code);
                confirmEmailChangeDto.Email = DecodeString(confirmEmailChangeDto.Email);

                var user = await FindByIdAsync(confirmEmailChangeDto.UserId);
                if (user == null)
                    return Response<ConfirmEmailChangeDto>.Fail("Unexpected error when trying to found user", 404);

                var result = await _userManager.ChangeEmailAsync(user, confirmEmailChangeDto.Email, confirmEmailChangeDto.Code);
                if (!result.Succeeded)
                    return Response<ConfirmEmailChangeDto>.Fail(result.Errors.Select(x => x.Description).ToList(), 404);

                return Response<ConfirmEmailChangeDto>.Fail("User's new email match current email", 404);
            }
            catch (Exception ex)
            {
                return Response<ConfirmEmailChangeDto>.Fail(ex.Message, 404);
            }
        }


        public async Task<Response<SendEmailConfirmationDto>> SendVerificationEmailAsync(SendEmailConfirmationDto sendEmailConfirmationDto)
        {
            try
            {
                if (sendEmailConfirmationDto.Email == null)
                    return Response<SendEmailConfirmationDto>.Fail("Unexpected error when trying to found user email", 404);

                var user = await _userManager.FindByEmailAsync(sendEmailConfirmationDto.Email);
                if (user == null)
                    return Response<SendEmailConfirmationDto>.Fail("Unexpected error when trying to found user", 404);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                sendEmailConfirmationDto.UserId = EncodeString(user.Id);
                sendEmailConfirmationDto.Code = EncodeString(code);

                return Response<SendEmailConfirmationDto>.Success(sendEmailConfirmationDto, 200);
            }
            catch (Exception ex)
            {
                return Response<SendEmailConfirmationDto>.Fail(ex.Message, 404);
            }
        }

        public async Task<Response<ConfirmEmailDto>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            try
            {
                if (confirmEmailDto.UserId == null)
                    return Response<ConfirmEmailDto>.Fail("Unexpected error when trying to found user", 404);

                var user = await FindByIdAsync(confirmEmailDto.UserId);

                if (user == null)
                    return Response<ConfirmEmailDto>.Fail("Unexpected error when trying to found user", 404);

                confirmEmailDto.UserId = DecodeString(confirmEmailDto.UserId);
                confirmEmailDto.Code = DecodeString(confirmEmailDto.Code);// Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmEmailDto.Code));
                var code = confirmEmailDto.Code;

                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (!result.Succeeded)
                    return Response<ConfirmEmailDto>.Fail(result.Errors.Select(x => x.Description).ToList(), 404);
                return Response<ConfirmEmailDto>.Success(confirmEmailDto, 200);
            }
            catch (Exception ex)
            {
                return Response<ConfirmEmailDto>.Fail(ex.Message, 404);
            }
        }




        #endregion

        #region EnableAuthenticator


        public async Task<Response<EnableAuthenticatorDto>> EnableAuthenticatorLoadSharedKeyAndQrCodeUriAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<EnableAuthenticatorDto>.Fail("Unexpected error when trying to found user", 404);

                var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(unformattedKey))
                {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                }
                EnableAuthenticatorDto enableAuthenticatorDto = new EnableAuthenticatorDto
                {
                    SharedKey = FormatKey(unformattedKey)
                };

                var email = await _userManager.GetEmailAsync(user);
                enableAuthenticatorDto.AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);

                if (enableAuthenticatorDto.SharedKey == null || enableAuthenticatorDto.AuthenticatorUri == null)
                    return Response<EnableAuthenticatorDto>.Fail($"User create enable authenticator key error", 404);

                return Response<EnableAuthenticatorDto>.Success(enableAuthenticatorDto, 200);
            }
            catch (Exception ex)
            {
                return Response<EnableAuthenticatorDto>.Fail(ex.Message, 404);
            }
        }

        public async Task<Response<EnableAuthenticatorDto>> VerifyAsync(EnableAuthenticatorDto enableAuthenticatorDto)
        {
            try
            {
                if (enableAuthenticatorDto.UserId == null)
                    return Response<EnableAuthenticatorDto>.Fail("Unexpected error when trying to found user", 404);

                var user = await FindByIdAsync(enableAuthenticatorDto.UserId);
                if (user == null)
                    return Response<EnableAuthenticatorDto>.Fail("Unexpected error when trying to found user", 404);

                var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, enableAuthenticatorDto.Code);
                if (!is2faTokenValid)
                    return Response<EnableAuthenticatorDto>.Fail("Verification code is invalid.", 404);

                await _userManager.SetTwoFactorEnabledAsync(user, true);

                var count = await _userManager.CountRecoveryCodesAsync(user);

                if (count == 0)
                {
                    var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                    enableAuthenticatorDto.RecoveryCodes = recoveryCodes.ToArray();
                }
                return Response<EnableAuthenticatorDto>.Success(enableAuthenticatorDto, 200);
            }
            catch (Exception ex)
            {
                return Response<EnableAuthenticatorDto>.Fail(ex.Message, 404);
            }
        }

        #region privateEnableAuthenticator

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                EnableAuthenticator.AuthenticatorUriFormat,
                _urlEncoder.Encode("FreeCourse.Web"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion

        #endregion

        #region TwoFactorAuthentication

        public async Task<Response<TwoFactorAuthenticationDto>> GetTwoFactorAuthenticationAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<TwoFactorAuthenticationDto>.Fail("Unexpected error when trying to found user", 404);

                TwoFactorAuthenticationDto twoFactorAuthenticationDto = new TwoFactorAuthenticationDto
                {
                    HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                    Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                    IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                    RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)

                };

                return Response<TwoFactorAuthenticationDto>.Success(twoFactorAuthenticationDto, 200);
            }
            catch (Exception ex)
            {
                return Response<TwoFactorAuthenticationDto>.Fail(ex.Message, 404);
            }
        }

        public async Task<Response<bool>> ForgetTwoFactorClientAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);

                await _signInManager.ForgetTwoFactorClientAsync();

                return Response<bool>.Success(true, 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(ex.Message, 404);
            }
        }

        #endregion

        #region ResetAuthenticator

        public async Task<Response<bool>> ResetAuthenticatorAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);

                var setResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                var resetResult = await _userManager.ResetAuthenticatorKeyAsync(user);

                await _signInManager.RefreshSignInAsync(user);
                if (setResult.Succeeded && resetResult.Succeeded)
                    return Response<bool>.Success(true, 200);
                else return Response<bool>.Success(false, 404);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(ex.Message, 404);
            }
        }

        #endregion

        #region GenerateRecoveryCodes

        public async Task<Response<GenerateRecoveryCodesDto>> GenerateRecoveryCodesAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<GenerateRecoveryCodesDto>.Fail("Unexpected error when trying to found user", 404);

                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

                if (recoveryCodes == null)
                    return Response<GenerateRecoveryCodesDto>.Fail("Generate recovery code error", 404);

                GenerateRecoveryCodesDto generateRecoveryCodesDto = new GenerateRecoveryCodesDto
                {
                    RecoveryCodes = recoveryCodes.ToArray(),
                    StatusMessage = "You have generated new recovery codes."
                };
                return Response<GenerateRecoveryCodesDto>.Success(generateRecoveryCodesDto, 200);
            }
            catch (Exception ex)
            {
                return Response<GenerateRecoveryCodesDto>.Fail(ex.Message, 404);
            }
        }

        #endregion

        #region ExternalLogins

        public async Task<Response<ExternalLoginsDto>> GetExternalLoginsAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<ExternalLoginsDto>.Fail("Unexpected error when trying to found user", 404);

                var currentLogins = await _userManager.GetLoginsAsync(user);
                var otherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                    .Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider))
                    .ToList();
                var showRemoveButton = user.PasswordHash != null || currentLogins.Count > 1;
                ExternalLoginsDto externalLoginsDto = new ExternalLoginsDto
                {
                    CurrentLogins = currentLogins,
                    OtherLogins = otherLogins,
                    ShowRemoveButton = showRemoveButton
                };

                return Response<ExternalLoginsDto>.Success(externalLoginsDto, 200);
            }
            catch (Exception ex)
            {
                return Response<ExternalLoginsDto>.Fail(ex.Message, 404);
            }
        }

        public async Task<Response<bool>> RemoveExternalLoginsAsync(RemoveExternalLoginDto removeExternalLoginDto)
        {
            try
            {
                if (removeExternalLoginDto.UserId == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);

                var user = await FindByIdAsync(removeExternalLoginDto.UserId);
                if (user == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);

                var result = await _userManager.RemoveLoginAsync(user, removeExternalLoginDto.LoginProvider, removeExternalLoginDto.ProviderKey);
                if (!result.Succeeded)
                    return Response<bool>.Fail("The external login remove error.", 404);
                await _signInManager.RefreshSignInAsync(user);
                return Response<bool>.Success(result.Succeeded, 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(ex.Message, 404);
            }
        }

        public async Task<Response<ChallengeResult>> LinkExternalLoginsAsync(LinkExternalLoginDto linkExternalLoginDto)
        {
            try
            {
                if (linkExternalLoginDto.UserId == null)
                    return Response<ChallengeResult>.Fail("Unexpected error when trying to found user", 404);

                var user = await FindByIdAsync(linkExternalLoginDto.UserId);
                if (user == null)
                    return Response<ChallengeResult>.Fail("Unexpected error when trying to found user", 404);

                await _signInManager.SignOutAsync();

                var host = _configuration.GetSection("JWT:ValidAudience");

                var callbackUrl = $"{host.Value}/Account/ExternalLogins/LinkLoginCallback";
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(linkExternalLoginDto.LoginProvider, callbackUrl, linkExternalLoginDto.UserId);

                return Response<ChallengeResult>.Success(new ChallengeResult(linkExternalLoginDto.LoginProvider, properties), 200);
            }
            catch (Exception ex)
            {
                return Response<ChallengeResult>.Fail(ex.Message, 404);
            }
        }


        public async Task<Response<bool>> LinkLoginCallbackAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);

                var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
                if (info == null)
                    return Response<bool>.Fail("User's external login getting error", 404);

                var result = await _userManager.AddLoginAsync(user, info);
                if (!result.Succeeded)
                    return Response<bool>.Fail("User's external login adding error", 404);

                await _signInManager.SignOutAsync();

                return Response<bool>.Success(result.Succeeded, 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(ex.Message, 404);
            }
        }


        #endregion

        #region UserProfile

        public async Task<Response<ApplicationUser>> GetUserProfileAsync(string userId)
        {
            try
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                    return Response<ApplicationUser>.Fail("Unexpected error when trying to found user", 404);
                return Response<ApplicationUser>.Success(user, 200);
            }
            catch (Exception ex)
            {
                return Response<ApplicationUser>.Fail(ex.Message, 404);
            }
        }

        public async Task<Response<bool>> UpdateProfileAsync(ApplicationUser applicationUser)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(applicationUser.Id);
                if (user == null)
                    return Response<bool>.Fail("Unexpected error when trying to found user", 404);


                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (applicationUser.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, applicationUser.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                        return Response<bool>.Fail("Unexpected error when trying to set phone number", 404);

                }
                var firstName = user.FirstName;
                var lastName = user.LastName;
                if (applicationUser.FirstName != firstName)
                {
                    user.FirstName = applicationUser.FirstName;
                    await _userManager.UpdateAsync(user);
                }
                if (applicationUser.LastName != lastName)
                {
                    user.LastName = applicationUser.LastName;
                    await _userManager.UpdateAsync(user);
                }
                if (user.UsernameChangeLimit > 0)
                {
                    if (applicationUser.UserName != user.UserName)
                    {
                        var userNameExists = await _userManager.FindByNameAsync(applicationUser.UserName);
                        if (userNameExists != null)
                            return Response<bool>.Fail("User name error. User name already taken. Select a different username.", 404);


                        var setUserName = await _userManager.SetUserNameAsync(user, applicationUser.UserName);
                        if (!setUserName.Succeeded)
                            return Response<bool>.Fail("Unexpected error when trying to set user name", 404);
                        else
                        {
                            user.UsernameChangeLimit -= 1;
                            await _userManager.UpdateAsync(user);
                        }
                    }
                }

                if (applicationUser.ProfilePicture != user.ProfilePicture)
                {
                    user.ProfilePicture = applicationUser.ProfilePicture;
                    await _userManager.UpdateAsync(user);
                }
                await _signInManager.RefreshSignInAsync(user);

                return Response<bool>.Success(true, 200);
            }
            catch (Exception ex)
            {
                return Response<bool>.Fail(ex.Message, 404);
            }
        }


        #endregion





        #region PrivateMethods

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);
            var expirationTimeUtc = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes);
            var localTimeZone = TimeZoneInfo.Local;
            var expirationTimeInLocalTimeZone = TimeZoneInfo.ConvertTimeFromUtc(expirationTimeUtc, localTimeZone);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expirationTimeInLocalTimeZone,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetClaimsPrincipal(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

            return principal;

        }


        private string EncodeString(string encodeString)
        {
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(encodeString));
        }

        private string DecodeString(string decodeString)
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(decodeString));
        }

        #endregion

    }

}
