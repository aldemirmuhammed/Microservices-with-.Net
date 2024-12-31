using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ExternalLogins;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.DotNet.Scaffolding.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class ExternalLoginsController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public ExternalLoginsController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData["ExternalLoginsStatus"] = null;
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/ExternalLogins/Index.cshtml");

            var result = await _ıdentityService.GetExternalLoginsAsync(userId);
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
            {
                result.Erros.ForEach(x => TempData["ExternalLoginsStatus"] += x);
                return View("~/Views/Account/ExternalLogins/Index.cshtml", result.Data);
            }
            return View("~/Views/Account/ExternalLogins/Index.cshtml", result.Data);
        }


        [HttpPost]
        public async Task<IActionResult> RemoveLogin(RemoveExternalLoginViewModel removeExternalLoginViewModel)
        {
            TempData["ExternalLoginsStatus"] = null;
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/ExternalLogins/Index.cshtml");

            removeExternalLoginViewModel.UserId = userId;
            var result = await _ıdentityService.RemoveExternalLoginsAsync(removeExternalLoginViewModel);
            if (!result.Data && (result.Erros != null && result.Erros.Any()))
            {
                result.Erros.ForEach(x => TempData["ExternalLoginsStatus"] += x);
                return View("~/Views/Account/ExternalLogins/Index.cshtml");
            }
            TempData["ExternalLoginsStatus"] = "The external logins removed successfully";
            return View("~/Views/Account/ExternalLogins/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            TempData["ExternalLoginsStatus"] = null;
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/ExternalLogins/Index.cshtml");
            LinkExternalLoginViewModel linkExternalLoginViewModel = new LinkExternalLoginViewModel();
            linkExternalLoginViewModel.UserId = userId;
            linkExternalLoginViewModel.LoginProvider = provider;
            var result = await _ıdentityService.LinkExternalLoginsAsync(linkExternalLoginViewModel);
            if (result.Data == null && (result.Erros != null && result.Erros.Any()))
            {
                result.Erros.ForEach(x => TempData["ExternalLoginsStatus"] += x);
                return View("~/Views/Account/ExternalLogins/Index.cshtml");
            }
            // return View("~/Views/Account/ExternalLogins/Index.cshtml", result.Data);

            return result.Data;
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            TempData["ExternalLoginsStatus"] = null;
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/ExternalLogins/Index.cshtml");

            var result = await _ıdentityService.LinkLoginCallbackAsync(userId);
            if (!result.Data || (result.Erros != null && result.Erros.Any()))
            {
                result.Erros.ForEach(x => TempData["ExternalLoginsStatus"] += x);
                return View("~/Views/Account/ExternalLogins/Index.cshtml");
            }

            TempData["ExternalLoginsStatus"] = "The external login adding operation is completed successfully.";
            return View("~/Views/Account/ExternalLogins/Index.cshtml");

        }
    }
}
