
using Microsoft.AspNetCore.Mvc;
using static IdentityServer4.IdentityServerConstants;
using User.Management.API.Models;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using User.Management.API.Dtos.SignUp;

namespace User.Management.API.Controllers
{
    // For protect controller authorize
    [Authorize(LocalApi.PolicyName)]
    //[Route("api/[controller]/[action]")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(SignupDto signupDto)
        {
            var user = new ApplicationUser
            {
                UserName = signupDto.UserName,
                Email = signupDto.Email,
                City = signupDto.City
            };
            var result = await _userManager.CreateAsync(user, signupDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));
            }
            return NoContent();
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null) return BadRequest();

            var user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null) return BadRequest();

            return Ok(new { Id = user.Id, UserName = user.UserName, Email = user.Email, City = user.City });
        }

        [HttpGet]
        [Route("api/[controller]/GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null) return BadRequest();

            return Ok(users);
        }



    }
}
