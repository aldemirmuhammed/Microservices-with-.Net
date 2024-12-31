using FreeCourse.Web.Models.Account.Admin.RoleManager;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly IRoleManagerService _roleManagerService;


        public UserRolesController(IRoleManagerService roleManagerService)
        {
            _roleManagerService = roleManagerService;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _roleManagerService.GetAllUserRoles());
        }

        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            var userList = await _roleManagerService.GetAllUserRoles();
            if (userList != null && userList.Count > 0)
            {
                var userName = userList.Where(x => x.UserId == userId).FirstOrDefault();
                if (userName != null)
                    ViewBag.UserName = userName.UserName;
            }
            return View(await _roleManagerService.ManageUserRoles(userId));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(List<ManageUserRolesViewModel> model, string userId)
        {
            var roleManage = await _roleManagerService.UpdateUserRoles(model, userId);
            return RedirectToAction("Index");
        }

    }
}
