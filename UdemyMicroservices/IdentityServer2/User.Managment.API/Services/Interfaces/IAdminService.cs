using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Identity;
using User.Management.API.Dtos.Authentication.User;

namespace User.Management.API.Services.Interfaces
{
    public interface IAdminService
    {

        Task<bool> RoleExistsAsync(string role);

        Task<Response<bool>> HasPermissionToRole(string userId);

        Task<Response<List<IdentityRole>>> GetIdentityRoles();

        Task<Response<UserRolesDto>> GetUserRolesByUserId(string userId);

        Task<Response<List<UserRolesDto>>> GetAllUserRoles();

        Task<Response<bool>> CreateRoleAsync(string roleName);

        Task<Response<bool>> DeleteRoleAsync(string roleId);

        Task<Response<List<ManageUserRolesDto>>> UpdateUserRoles(List<ManageUserRolesDto> model, string userId);

        Task<Response<List<ManageUserRolesDto>>> ManageUserRoles(string userId);

    }
}
