using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using User.Management.API.Dtos.Authentication.User;
using User.Management.API.Models;
using User.Management.API.Services.Interfaces;

namespace User.Management.API.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AdminService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

        public async Task<Response<bool>> HasPermissionToRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Response<bool>.Fail("User not found", 404);

            var userRoles = await GetUserRoles(user);
            if (userRoles == null)
                return Response<bool>.Fail("User roles not found", 404);

            if (!userRoles.Any(x => x.Equals(Enum.Roles.Admin.ToString())))
                return Response<bool>.Fail("User roles not found permission", 404);

            return Response<bool>.Success(true, 200);

        }

        public async Task<Response<List<IdentityRole>>> GetIdentityRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            if (roles == null)
                return Response<List<IdentityRole>>.Fail("Roles not found", 404);

            return Response<List<IdentityRole>>.Success(roles, 200);
        }

        public async Task<Response<UserRolesDto>> GetUserRolesByUserId(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Response<UserRolesDto>.Fail("User not found", 404);

            var userRolesVM = new UserRolesDto();
            userRolesVM.UserId = user.Id;
            userRolesVM.Email = user.Email;
            userRolesVM.UserName = user.UserName;
            userRolesVM.Roles = await GetUserRoles(user);

            return Response<UserRolesDto>.Success(userRolesVM, 200);
        }

        // Get all users and user's roles
        public async Task<Response<List<UserRolesDto>>> GetAllUserRoles()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null)
                return Response<List<UserRolesDto>>.Fail("User roles not fund.", 404);
            var userRolesViewModel = new List<UserRolesDto>();
            foreach (ApplicationUser user in users)
            {
                var thisViewModel = new UserRolesDto();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.UserName = user.UserName;
                thisViewModel.Roles = await GetUserRoles(user);

                userRolesViewModel.Add(thisViewModel);
            }
            return Response<List<UserRolesDto>>.Success(userRolesViewModel, 200);
        }

        public async Task<Response<bool>> CreateRoleAsync(string roleName)
        {
            if (roleName != null)
            {
                var role = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
                if (role != null && role.Succeeded)
                    return Response<bool>.Success(role.Succeeded, 200);
            }
            return Response<bool>.Fail("Role could not create", 404);
        }

        public async Task<Response<bool>> DeleteRoleAsync(string roleId)
        {
            if (roleId != null)
            {
                var role = await _roleManager.Roles.Where(x => x.Id == roleId).FirstOrDefaultAsync();
                if (role == null)
                    return Response<bool>.Fail("Role could not found", 404);
                var result = await _roleManager.DeleteAsync(role);
                if (result != null && result.Succeeded)
                    return Response<bool>.Success(result.Succeeded, 200);
            }
            return Response<bool>.Fail("Role could not delete", 404);
        }

        public async Task<Response<List<ManageUserRolesDto>>> UpdateUserRoles(List<ManageUserRolesDto> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Response<List<ManageUserRolesDto>>.Fail("User not found", 404);

            }
            var roles = await _userManager.GetRolesAsync(user);

            if (roles == null)
                return Response<List<ManageUserRolesDto>>.Fail("Roles not found", 404);

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
                return Response<List<ManageUserRolesDto>>.Fail("Cannot remove user existing roles", 404);

            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
                return Response<List<ManageUserRolesDto>>.Fail("Cannot add selected roles to user", 404);

            return Response<List<ManageUserRolesDto>>.Success(model, 200);
        }

        public async Task<Response<List<ManageUserRolesDto>>> ManageUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Response<List<ManageUserRolesDto>>.Fail("User not found", 404);

            var rolesList = _roleManager.Roles;
            if (rolesList == null)
                return Response<List<ManageUserRolesDto>>.Fail("Roles not found", 404);

            var model = new List<ManageUserRolesDto>();

            var userRoleList = await GetUserRolesByUserId(userId);
            if (userRoleList == null || userRoleList.Data.Roles == null || userRoleList.Data.Roles.Count() == 0)
                return Response<List<ManageUserRolesDto>>.Fail("User roles not found", 404);


            foreach (var role in rolesList)
            {

                var _isUserExistInRole = userRoleList.Data.Roles.Where(x => x == role.Name).FirstOrDefault();
                if (_isUserExistInRole != null)
                {
                    var userRolesViewModel = new ManageUserRolesDto
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };
                    userRolesViewModel.Selected = true;
                    model.Add(userRolesViewModel);
                }
            }
            return Response<List<ManageUserRolesDto>>.Success(model, 200);
        }



        #region PrivateMethods

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        #endregion
    }
}
