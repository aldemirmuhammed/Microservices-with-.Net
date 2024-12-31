using FreeCourse.Web.Models.Account.Admin.RoleManager;
using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class RoleManagerController : Controller
    {

        private readonly IRoleManagerService _roleManagerService;

        public RoleManagerController(IRoleManagerService roleManagerService)
        {
            _roleManagerService = roleManagerService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _roleManagerService.GetIdentityRoles());
        }

        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
                await _roleManagerService.CreateRoleAsync(roleName);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (roleId != null)
                await _roleManagerService.DeleteRoleAsync(roleId);
            return RedirectToAction(nameof(Index));
        }


    }
}
