using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {

        private readonly IUserService _userServicecs;

        public UserController(IUserService userServicecs)
        {
            _userServicecs = userServicecs;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userServicecs.GetUser());
        }
    }
}
