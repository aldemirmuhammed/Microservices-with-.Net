using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class ProfileController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public ProfileController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData["UserNameChangeLimitMessage"] = null;
            TempData["ProfileStatusMessage"] = null;

            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/Profile/Index.cshtml");


            var user = await LoadAsync(userId);
            if (user == null)
                return View("~/Views/Account/Profile/Index.cshtml");
            TempData["UserNameChangeLimitMessage"] = $"You can change your username {user.UsernameChangeLimit} more time(s).";

            return View("~/Views/Account/Profile/Index.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ApplicationUser applicationUser)
        {
            TempData["UserNameChangeLimitMessage"] = null;
            TempData["ProfileStatusMessage"] = null;

            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/Profile/Index.cshtml");

            if(applicationUser == null)
                return View("~/Views/Account/Profile/Index.cshtml");

            var user = await LoadAsync(userId);
            if(user == null)
                return View("~/Views/Account/Profile/Index.cshtml");

          
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    applicationUser.ProfilePicture =  dataStream.ToArray();                 
                }
            }

            applicationUser.Id = user.Id;
            var result = await _ıdentityService.UpdateProfileAsync(applicationUser);
            if (result.Erros != null && result.Erros.Any())
            {
                result.Erros.ForEach(x => TempData["ProfileStatusMessage"] += x);
                return View("~/Views/Account/Profile/Index.cshtml");
            }

            TempData["ProfileStatusMessage"] = "Your profile has been updated successfully";
            return View("~/Views/Account/Profile/Index.cshtml");
        }

        private async Task<ApplicationUser> LoadAsync(string userId)
        {
            if (userId == null)
                return null;
            var profile = await _ıdentityService.GetUserProfileAsync(userId);
            if (profile == null)
                return null;

            return profile.Data;
        }
    }
}
