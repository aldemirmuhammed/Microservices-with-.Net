using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ConfirmEmailChange;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class ConfirmEmailChangeController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public ConfirmEmailChangeController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id, string email, string code)
        {
            TempData["ConfirmEmailChangeStatusMessage"] = null;
            if (id == null || email == null || code == null)
                return View("~/Views/Account/ConfirmEmailChange/Index.cshtml");

            var confirmEmailChangeViewModel = new ConfirmEmailChangeViewModel
            {
                Code = code,
                Email = email,
                UserId = id
            };
            var result = await _ıdentityService.ConfirmEmailChangeAsync(confirmEmailChangeViewModel);
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
            {
                result.Erros.ForEach(x => TempData["ConfirmEmailChangeStatusMessage"] += x);
                return View("~/Views/Account/ConfirmEmailChange/Index.cshtml");
            }

            TempData["ConfirmEmailChangeStatusMessage"] = "Email confirmation completed successfully.";
            return View("~/Views/Account/ConfirmEmailChange/Index.cshtml");
        }
    }
}
