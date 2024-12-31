using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.ConfirmEmail;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class ConfirmEmailController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public ConfirmEmailController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id, string token, string returnUrl = null)
        {
            if (id == null || token == null)
                return View("~/Views/Account/ConfirmEmail/Index.cshtml");

            ConfirmEmailViewModel confirmEmailViewModel = new ConfirmEmailViewModel
            {
                UserId = id,
                Code = token
            };
            var result = await _ıdentityService.ConfirmEmailAsync(confirmEmailViewModel);
            if (result.Data == null && result.Erros != null && result.Erros.Any())
            {
                result.Erros.ForEach(x => TempData["ConfirmEmailStatus"] += x);
                return View("~/Views/Account/ConfirmEmail/Index.cshtml");
            }
            TempData["ConfirmEmailStatus"] = "Email comfirmation completed successfully";

            return View("~/Views/Account/ConfirmEmail/Index.cshtml");
        }
    }
}
