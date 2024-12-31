using FreeCourse.Web.Models.Account.Admin.RoleManager;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IRoleManagerService
    {
        Task<bool> HasPermissionToRole();


        Task<bool> CreateRoleAsync(string role);

        Task<bool> DeleteRoleAsync(string roleId);

        Task<List<UserRolesViewModel>> GetAllUserRoles();

        Task<List<IdentityRole>> GetIdentityRoles();

        Task<List<ManageUserRolesViewModel>> ManageUserRoles(string userId);

        Task<List<ManageUserRolesViewModel>> UpdateUserRoles(List<ManageUserRolesViewModel> model, string userId);

    }
}
