using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User.Management.API.Dtos.Authentication.User;
using User.Management.API.Models;
using User.Management.API.Services.Interfaces;
using static IdentityServer4.IdentityServerConstants;

namespace User.Management.API.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : CustomBaseController
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(IAdminService adminService, UserManager<ApplicationUser> userManager)
        {
            _adminService = adminService;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("/api/[controller]/HasPermissionToRole/{userId}")]
        public async Task<IActionResult> HasPermissionToRole(string userId)
        {
            var result = await _adminService.HasPermissionToRole(userId);
            return CreateActionResultInstance(result);
        }

        [HttpGet]
        [Route("/api/[controller]/GetIdentityRoles")]
        public async Task<IActionResult> GetIdentityRoles()
        {
            return CreateActionResultInstance(await _adminService.GetIdentityRoles());
        }

        [HttpGet]
        [Route("/api/[controller]/GetUserRolesByUserId/{userId}")]
        public async Task<IActionResult> GetUserRolesByUserId(string userId)
        {
            return CreateActionResultInstance(await _adminService.GetUserRolesByUserId(userId));
        }

        [HttpGet]
        [Route("/api/[controller]/GetAllUserRoles")]
        public async Task<IActionResult> GetUserRoles()
        {
            return CreateActionResultInstance(await _adminService.GetAllUserRoles());
        }

        #region ManagerRoles

        [HttpPost]
        [Route("/api/[controller]/CreateRoleAsync/{roleName}")]

        public async Task<IActionResult> CreateRoleAsync(string roleName)
        {
            return CreateActionResultInstance(await _adminService.CreateRoleAsync(roleName));
        }

        [HttpDelete]
        [Route("/api/[controller]/DeleteRoleAsync/{roleId}")]

        public async Task<IActionResult> DeleteRoleAsync(string roleId)
        {
            return CreateActionResultInstance(await _adminService.DeleteRoleAsync(roleId));
        }

        [HttpGet]
        [Route("/api/[controller]/ManageUserRoles/{userId}")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            return CreateActionResultInstance(await _adminService.ManageUserRoles(userId));
        }

        [HttpGet]
        [Route("/api/[controller]/UpdateUserRoles/{userId}")]
        public async Task<IActionResult> UpdateUserRoles(List<ManageUserRolesDto> model, string userId)
        {
            return CreateActionResultInstance(await _adminService.UpdateUserRoles(model, userId));
        }



        #endregion
    }
}
