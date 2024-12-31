using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.EnableAuthenticator;
using FreeCourse.Web.Models.Account.ShowRecoveryCodes;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class EnableAuthenticatorController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public EnableAuthenticatorController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = _sharedIdentityService.GetUserId;

            var result = await _ıdentityService.LoadSharedKeyAndQrCodeUriAsync(userId);
            if (result.Data == null && result.Erros != null && result.Erros.Any())
                result.Erros.ForEach(r => { TempData["EnableAuthenticatorStatus"] += r + "/n"; });

            return View("~/Views/Account/EnableAuthenticator/Index.cshtml", result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Verify(EnableAuthenticatorViewModel enableAuthenticatorViewModel)
        {
            TempData["EnableAuthenticatorStatus"] = null;
            string userId = _sharedIdentityService.GetUserId;

            if (enableAuthenticatorViewModel == null || enableAuthenticatorViewModel.Code == null)
            {
                var result = await _ıdentityService.LoadSharedKeyAndQrCodeUriAsync(userId);
                if (result.Data == null && result.Erros != null && result.Erros.Any())
                    result.Erros.ForEach(r => { TempData["EnableAuthenticatorStatus"] += r + "/n"; });
                return View("~/Views/Account/EnableAuthenticator/Index.cshtml");

            }

            var verificationCode = enableAuthenticatorViewModel.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            enableAuthenticatorViewModel.Code = verificationCode;
            enableAuthenticatorViewModel.UserId = userId;

            var verify = await _ıdentityService.VerifyAsync(enableAuthenticatorViewModel);

            if (verify.Data == null && verify.Erros != null && verify.Erros.Any())
            {
                await _ıdentityService.LoadSharedKeyAndQrCodeUriAsync(userId);
                verify.Erros.ForEach(r => { TempData["EnableAuthenticatorStatus"] += r + "/n"; });
                return View("~/Views/Account/EnableAuthenticator/Index.cshtml");
            }

            if (verify.Data.RecoveryCodes != null)
            {
                ShowRecoveryCodesViewModel showRecoveryCodesViewModel = new ShowRecoveryCodesViewModel
                {
                    RecoveryCodes = verify.Data.RecoveryCodes
                };
                return View("~/Views/Account/ShowRecoveryCodes/Index.cshtml", showRecoveryCodesViewModel);
            
            }
            else
                return View("~/Views/Account/TwoFactorAuthentication/Index.cshtml");

        }
    }
}
