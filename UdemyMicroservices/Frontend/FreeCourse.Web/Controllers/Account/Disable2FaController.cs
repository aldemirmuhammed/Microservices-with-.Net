using FreeCourse.Shared.Services;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace FreeCourse.Web.Controllers.Account
{
    public class Disable2FaController : Controller
    {

        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public Disable2FaController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<IActionResult> Index()
        {
            TempData["TwoFactorEnabledStatus"] = null;
            var userId = _sharedIdentityService.GetUserId;
            var result = await _ıdentityService.SetTwoFactorEnabledAsync(userId);

            if (!result.Data && result.Erros != null)
                result.Erros.ForEach(x => TempData["TwoFactorEnabledStatus"] += x);
            return View("~/Views/Account/Disable2Fa/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> SetTwoFactorEnabledAsync()
        {
            TempData["TwoFactorEnabledStatus"] = null;
            var userId = _sharedIdentityService.GetUserId;
            var disable2faResult = await _ıdentityService.SetTwoFactorEnabledAsync(userId);

            if (!disable2faResult.Data && disable2faResult.Erros != null)
            {
                disable2faResult.Erros.ForEach(x => TempData["TwoFactorEnabledStatus"] += x);
                return View("~/Views/Account/Disable2Fa/Index.cshtml");
            }
            return View("~/Views/Account/TwoFactorAuthentication/Index.cshtml");
        }
    }
}
