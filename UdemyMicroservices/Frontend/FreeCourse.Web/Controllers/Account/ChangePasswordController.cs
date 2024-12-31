using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ChangePassword;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class ChangePasswordController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public ChangePasswordController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/ChangePassword/Index.cshtml");


            var hasPassword = await _ıdentityService.HasPasswordAsync(userId);

            if (!hasPassword.Data)
                return View("~/Views/Account/SetPassword/Index.cshtml");

            return View("~/Views/Account/ChangePassword/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            TempData["ChangePasswordStatus"] = null;
            string userId = _sharedIdentityService.GetUserId;
            if (userId == null)
            {
                TempData["ChangePasswordStatus"] =  "User have not null";
                return RedirectToAction(nameof(Index));
            }
            if (changePasswordViewModel == null
                || changePasswordViewModel.OldPassword == null
                || changePasswordViewModel.NewPassword == null
                || changePasswordViewModel.ConfirmPassword == null)
            {
                TempData["ChangePasswordStatus"] = "Change password parameter can not empty";
                return RedirectToAction(nameof(Index));
            }

            changePasswordViewModel.UserId = userId;

            var result = await _ıdentityService.ChangePasswordAsync(changePasswordViewModel);
            if (result.Data == null && result.Erros != null)
                result.Erros.ForEach(x => TempData["ChangePasswordStatus"] += x);

            return RedirectToAction(nameof(Index));
        }



    }
}
