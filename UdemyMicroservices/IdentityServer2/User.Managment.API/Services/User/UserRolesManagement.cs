using Microsoft.AspNetCore.Identity;
using User.Management.API.Models;
using User.Management.API.Services.Interfaces;

namespace User.Management.API.Services.User
{
    public class UserRolesManagement : IUserRolesManagement
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserRolesManagement(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

    

    }

}
