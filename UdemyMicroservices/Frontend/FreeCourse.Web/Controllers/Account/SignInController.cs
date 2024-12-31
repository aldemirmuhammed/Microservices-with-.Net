using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ChangePassword;
using FreeCourse.Web.Models.Account.ResetPassword;
using FreeCourse.Web.Models.Account.SignIn;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class SignInController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public SignInController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Account/SignIn/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SigninInput signinInput)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account/SignIn/Index.cshtml");


            var result = await _ıdentityService.LoginAsync(signinInput);

            if (result.Data == null && result.Erros != null)
            {
                result.Erros.ForEach(x => { ModelState.AddModelError(string.Empty, x); });
                return View("~/Views/Account/SignIn/Index.cshtml");
            }
            return RedirectToAction(nameof(Index), "Home");

        }
    }
}
