using FreeCourse.Shared.Services;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    [AllowAnonymous]
    public class LockoutController : Controller
    {
        public LockoutController()
        {
           
        }
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Account/Lockout/Index.cshtml");
        }
    }
}
