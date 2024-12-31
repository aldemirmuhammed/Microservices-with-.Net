using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.RegisterConfirmation;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class RegisterConfirmationController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public RegisterConfirmationController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string email, string returnUrl = null)
        {
            if (email == null)
                return NotFound($"Unable to load user.");

            TempData["Email"] = email;
            TempData["DisplayConfirmAccountLink"] = true;
            if ((bool)TempData["DisplayConfirmAccountLink"])
            {

                var result = await _ıdentityService.GenerateEmailConfirmationTokenAsync(new RegisterConfirmationViewModel { Email = email, Code = "Code" });
                if (result.Data == null || result.Data.Code == null)
                    return NotFound($"Unable to trust code.");

                
                TempData["EmailConfirmationUrl"] = Url.ActionLink(
                    nameof(Index),
                    "ConfirmEmail",
                    values: new { id = result.Data.UserId, token = result.Data.Code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
            }
            return View("~/Views/Account/RegisterConfirmation/Index.cshtml");
        }
    }
}
