using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.GenerateRecoveryCodes;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class GenerateRecoveryCodesController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public GenerateRecoveryCodesController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData["GenerateRecoveryCodesStatus"] = null;
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/GenerateRecoveryCodes/Index.cshtml");

            var result = await _ıdentityService.GetTwoFactorEnabledAsync(userId);
            if (!result.Data || (result.Erros != null && result.Erros.Any()))
            {
                result.Erros.ForEach(x => TempData["GenerateRecoveryCodesStatus"] += x);
                return View("~/Views/Account/GenerateRecoveryCodes/Index.cshtml");
            }
            return View("~/Views/Account/GenerateRecoveryCodes/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            TempData["GenerateRecoveryCodesStatus"] = null;

            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/GenerateRecoveryCodes/Index.cshtml");
            var result = await _ıdentityService.GetTwoFactorEnabledAsync(userId);
            if (!result.Data || (result.Erros != null && result.Erros.Any()))
            {
                result.Erros.ForEach(x => TempData["GenerateRecoveryCodesStatus"] += x);
                return View("~/Views/Account/GenerateRecoveryCodes/Index.cshtml");
            }

            var recoveryCodes = await _ıdentityService.GenerateRecoveryCodesAsync(userId);
            if (recoveryCodes.Data == null && (recoveryCodes.Erros != null && recoveryCodes.Erros.Any()))
            {
                result.Erros.ForEach(x => TempData["GenerateRecoveryCodesStatus"] += x);
            }

            return View("~/Views/Account/GenerateRecoveryCodes/Index.cshtml");

        }
    }
}
