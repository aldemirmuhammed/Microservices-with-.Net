using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.TwoFactorAuthentication;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class TwoFactorAuthenticationController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public TwoFactorAuthenticationController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/TwoFactorAuthentication/Index.cshtml");

            var result = await _ıdentityService.GetTwoFactorAuthenticationAsync(userId);

            TempData["TwoFactorAuthenticationStatusMessage"] = null;

            if (result.Data == null && result.Erros != null && result.Erros.Any())
            {
                result.Erros.ForEach(x => TempData["TwoFactorAuthenticationStatusMessage"] += "\n" + x);
            }
            return View("~/Views/Account/TwoFactorAuthentication/Index.cshtml", result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> ForgetTwoFactorClient(TwoFactorAuthenticationViewModel twoFactorAuthenticationViewModel)
        {
            string userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/TwoFactorAuthentication/Index.cshtml");

            var result = await _ıdentityService.ForgetTwoFactorClientAsync(userId);

            TempData["TwoFactorAuthenticationStatusMessage"] = null;

            if (!result.Data && result.Erros != null && result.Erros.Any())
                result.Erros.ForEach(x => TempData["TwoFactorAuthenticationStatusMessage"] += "\n" + x);
            else
                TempData["TwoFactorAuthenticationStatusMessage"] = "Operation completed successfully";

            return View("~/Views/Account/TwoFactorAuthentication/Index.cshtml");
        }
    }
}
