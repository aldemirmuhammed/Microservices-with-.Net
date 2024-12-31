using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers.Account
{
    public class AccessDeniedController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Account/AccessDenied/Index.cshtml");
        }
    }
}
