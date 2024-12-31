using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ShowRecoveryCodes;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers.Account
{
    public class ShowRecoveryCodesController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public ShowRecoveryCodesController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }
        public IActionResult Index(ShowRecoveryCodesViewModel showRecoveryCodesViewModel)
        {
            if(showRecoveryCodesViewModel == null || showRecoveryCodesViewModel.RecoveryCodes == null)
                return View("~/Views/Account/TwoFactorAuthentication/Index.cshtml");

            return View("~/Views/Account/ShowRecoveryCodes/Index.cshtml");
        }
    }
}
