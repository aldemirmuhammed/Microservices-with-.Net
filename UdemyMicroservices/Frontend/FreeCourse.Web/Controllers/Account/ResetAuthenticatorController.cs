using FreeCourse.Shared.Services;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class ResetAuthenticatorController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public ResetAuthenticatorController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData["ResetAuthenticatorStatus"] = null;
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                TempData["ResetAuthenticatorStatus"] = "An error occurred while getting user.";
            return View("~/Views/Account/ResetAuthenticator/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ResetAuthenticator()
        {
            TempData["ResetAuthenticatorStatus"] = null;
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/ResetAuthenticator/Index.cshtml");

            var result = await _ıdentityService.ResetAuthenticatorAsync(userId);
            if (!result.Data && result.Erros != null && result.Erros.Any())
            {
                result.Erros.ForEach(x => TempData["ResetAuthenticatorStatus"] += x);
                return View("~/Views/Account/ResetAuthenticator/Index.cshtml");
            }

            return View("~/Views/Account/EnableAuthenticator/Index.cshtml");
        }
    }
}
