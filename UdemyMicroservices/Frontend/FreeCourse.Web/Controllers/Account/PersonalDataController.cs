using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account;
using FreeCourse.Web.Models.Account.PersonalData;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers.Account
{
    public class PersonalDataController : Controller
    {
        private readonly IIdentityService _ıdentityService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public PersonalDataController(IIdentityService ıdentityService, ISharedIdentityService sharedIdentityService)
        {
            _ıdentityService = ıdentityService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return View("~/Views/Account/PersonalData/Index.cshtml");
            var hasPassword = await _ıdentityService.HasPasswordAsync(userId);
            TempData["RequirePassword"] = hasPassword.Data;
            return View("~/Views/Account/PersonalData/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            return View("~/Views/Account/PersonalData/Delete.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeletePersonalDataViewModel deletePersonalDataViewModel)
        {
            TempData["RequirePassword"] = null;
            TempData["DeletePersonalDataStatus"] = null;
            if (deletePersonalDataViewModel == null)
                return View("~/Views/Account/PersonalData/Index.cshtml");

            deletePersonalDataViewModel.UserId = _sharedIdentityService.GetUserId;
            var result = await _ıdentityService.DeletePersonalDataAsync(deletePersonalDataViewModel);

            if (result.Erros != null && result.Erros.Any())
            {
                TempData["DeletePersonalDataStatus"] = "Error";
                result.Erros.ForEach(x => TempData["DeletePersonalDataStatus"] += "\n" + x);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Download()
        {
            DownloadPersonalDataViewModel downloadPersonalDataViewModel = new DownloadPersonalDataViewModel();
            TempData["DownloadPersonalDataStatus"] = null;
            downloadPersonalDataViewModel.UserId = _sharedIdentityService.GetUserId;
            var result = await _ıdentityService.DownloadPersonalDataAsync(downloadPersonalDataViewModel);

            if (result.Erros != null && result.Erros.Any())
                result.Erros.ForEach(x => TempData["DownloadPersonalDataStatus"] += "\n" + x);
            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(result.Data), "application/json");
        }
    }
}
