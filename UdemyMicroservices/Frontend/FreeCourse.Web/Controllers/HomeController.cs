using FreeCourse.Shared.Services;
using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.Catalogs.Course.CourseComment;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICatalogService _catalogService;
        private readonly ISharedIdentityService _sharedIdentityService;
        public HomeController(ILogger<HomeController> logger, ICatalogService catalogService, ISharedIdentityService sharedIdentityService)
        {
            _logger = logger;
            _catalogService = catalogService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<IActionResult> Index()
        {
            var course = await _catalogService.GetAllCourseAsync();
            if (course != null && _sharedIdentityService.GetUserId != string.Empty)
            {
                // Get user fav list
                var favlist = await _catalogService.GetFavoriteCoursesAsync(_sharedIdentityService.GetUserId);

                List<decimal> rateList = new();
                foreach (var item in course)
                {

                    if (favlist != null && favlist.Count > 0)
                    {
                        var isExist = favlist.Any(x => x.CourseId == item.Id);
                        if (isExist)
                            item.IsFavorite = true;
                    }

                    // Get course's all comment 
                    var commentList = await _catalogService.GetAllCommentByCourseIdAsync(item.Id);
                    if (commentList != null && commentList.Data != null && commentList.Data.Count > 0)
                        item.CourseCommentList = commentList.Data;

                    // Get course's all questions 
                    var questionsList = await _catalogService.GetQuestionsByCourseIdAsync(item.Id);
                    if (questionsList != null && questionsList.Data != null && questionsList.Data.Count > 0)
                        item.CourseQuestionsList = questionsList.Data;
                }

            }
            return View(course);
        }


        public async Task<IActionResult> Detail(string id)
        {
            var course = await _catalogService.GetByCourseId(id);

            if (course != null && _sharedIdentityService.GetUserId != string.Empty)
            {
                List<decimal> rateList = new();


                var favlist = await _catalogService.GetFavoriteCoursesAsync(_sharedIdentityService.GetUserId);
                if (favlist != null && favlist.Count > 0)
                {
                    var isExist = favlist.Any(x => x.CourseId == course.Id);
                    if (isExist)
                    {
                        course.IsFavorite = true;
                    }

                    // Get course's all comment 
                    var commentList = await _catalogService.GetAllCommentByCourseIdAsync(course.Id);
                    if (commentList != null && commentList.Data != null && commentList.Data.Count > 0)
                    {
                        course.CourseCommentList = commentList.Data;
                    }

                    // Get course's all questions 
                    var questionsList = await _catalogService.GetQuestionsByCourseIdAsync(course.Id);
                    if (questionsList != null && questionsList.Data != null && questionsList.Data.Count > 0)
                        course.CourseQuestionsList = questionsList.Data;
                }
            }
            return View(course);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (errorFeature != null && errorFeature.Error is UnAuthorizeException)
            {
                return RedirectToAction(nameof(AuthController.Logout), "Auth");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
