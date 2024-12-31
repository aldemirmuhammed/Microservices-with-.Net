using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ChangePassword;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class SetPasswordController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public SetPasswordController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/SetPassword/Index.cshtml");

            var hasPassword = await _ıdentityService.HasPasswordAsync(userId);

            if (hasPassword.Data)
                return View("~/Views/Account/ChangePassword/Index.cshtml");

            return View("~/Views/Account/SetPassword/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel setPasswordViewModel)
        {
            TempData["SetPasswordStatus"] = null;

            string userId = _sharedIdentityService.GetUserId;
            if (userId == null)
            {
                TempData["SetPasswordStatus"] = "User have not null";
                return RedirectToAction(nameof(Index));
            }
            if (setPasswordViewModel == null)
            {
                TempData["SetPasswordStatus"] = "Set password parameter can not empty";
                return RedirectToAction(nameof(Index));
            }

            setPasswordViewModel.UserId = userId;

            var result = await _ıdentityService.SetPasswordAsync(setPasswordViewModel);

            if (result.Data == null && result.Erros != null)
                result.Erros.ForEach(x => TempData["SetPasswordStatus"] += x);

            return RedirectToAction(nameof(Index));
        }


    }
}
