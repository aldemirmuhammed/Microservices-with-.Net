using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Catalogs;
using FreeCourse.Web.Models.Catalogs.Course.CourseComment;
using FreeCourse.Web.Models.Catalogs.Course.CourseQuestion;
using FreeCourse.Web.Models.Catalogs.Course.CourseUser;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly ISharedIdentityService _sharedIdentityService;
        private readonly IIdentityService _identityService;

        public CoursesController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService, IIdentityService identityService)
        {
            _catalogService = catalogService;
            _sharedIdentityService = sharedIdentityService;
            _identityService = identityService;
        }

        #region Course

        public async Task<IActionResult> Index()
        {
            return View(await _catalogService.GetAllCourseByUserIdAsync(_sharedIdentityService.GetUserId));
        }

        public async Task<IActionResult> Create()
        {

            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.categoryList = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateInput courseCreateInput)
        {
            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.categoryList = new SelectList(categories, "Id", "Name");
            if (!ModelState.IsValid)
            {
                return View();
            }
            courseCreateInput.UserId = _sharedIdentityService.GetUserId;
            await _catalogService.CreateCourseAsync(courseCreateInput);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(string id)
        {

            var course = await _catalogService.GetByCourseId(id);
            var categories = await _catalogService.GetAllCategoryAsync();

            if (course == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.categoryList = new SelectList(categories, "Id", "Name", course.Id);

            CourseUpdateInput courseUpdateInput = new CourseUpdateInput
            {
                Id = course.Id,
                Name = course.Name,
                Price = course.Price,
                Feature = course.Feature,
                CategoryId = course.CategoryId,
                Description = course.Description,
                Picture = course.Picture,
                UserId = course.UserId

            };
            return View(courseUpdateInput);
        }


        [HttpPost]
        public async Task<IActionResult> Update(CourseUpdateInput courseUpdateInput)
        {
            var categories = await _catalogService.GetAllCategoryAsync();
            ViewBag.categoryList = new SelectList(categories, "Id", "Name", courseUpdateInput.Id);


            if (!ModelState.IsValid)
            {
                return View();
            }
            await _catalogService.UpdateCourseAsync(courseUpdateInput);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(string id)
        {
            await _catalogService.DeleteCourseAsync(id);
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region CourseFavorite


        public async Task<IActionResult> FavoriteCourses()
        {
            return View(await _catalogService.GetAllFavoriteCourseByUserIdAsync(_sharedIdentityService.GetUserId));
        }

        public async Task<IActionResult> CreateFavoriteCourse(string courseId)
        {
            FavoriteCourseViewModel favoriteCourseViewModel = new FavoriteCourseViewModel
            {
                CourseId = courseId,
                UserId = _sharedIdentityService.GetUserId,
                CreatedTime = DateTime.Now
            };
            await _catalogService.CreateFavoriteCourseAsync(favoriteCourseViewModel);
            return RedirectToAction(nameof(FavoriteCourses));
        }

        public async Task<IActionResult> DeleteFavoriteCourse(string courseId)
        {
            var delete = new DeleteFavoriteCourseViewModel { CourseId = courseId, UserId = _sharedIdentityService.GetUserId };
            await _catalogService.DeleteFavoriteCourseAsync(delete);
            return RedirectToAction(nameof(FavoriteCourses));
        }

        #endregion

        #region CourseComment

        public async Task<IActionResult> Comment()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CommentCreate()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CommentCreate(CourseCommentViewModel courseCommentViewModel)
        {

            if (_sharedIdentityService == null || _sharedIdentityService.GetUserId == null)
                return RedirectToAction("Detail", "Home", routeValues: new { id = courseCommentViewModel.CourseId });


            var user = await _identityService.GetUserProfileAsync(_sharedIdentityService.GetUserId);
            if (user != null || user.Data != null)
            {
                var usr = new CourseUserViewModel
                {
                    Id = user.Data.Id,
                    FirstName = user.Data.FirstName,
                    LastName = user.Data.LastName,
                    Email = user.Data.Email,
                    PhoneNumber = user.Data.PhoneNumber,
                    City = user.Data.City,
                    ProfilePicture = user.Data.ProfilePicture,
                    UserName = user.Data.UserName
                };
                courseCommentViewModel.User = usr;
                courseCommentViewModel.CreatedTime = DateTime.Now;
                var result = await _catalogService.CreateCommentCourseAsync(courseCommentViewModel);


                return RedirectToAction("Detail", "Home", routeValues: new { id = courseCommentViewModel.CourseId });
            }

            //var result = await _catalogService.GetByCourseId(courseId);
            return RedirectToAction("Detail", "Home", routeValues: new { id = courseCommentViewModel.CourseId });
        }

        public async Task<IActionResult> LikeComment(string commentId, string courseId)
        {

            var result = await _catalogService.LikeCommentCourseAsync(commentId);
            var course = await _catalogService.GetByCourseId(courseId);
            return RedirectToAction("Detail", "Home", routeValues: new { id = course.Id });
        }

        public async Task<IActionResult> DislikeComment(string commentId, string courseId)
        {
            var result = await _catalogService.DislikeCommentCourseAsync(commentId);
            var course = await _catalogService.GetByCourseId(courseId);

            return RedirectToAction("Detail", "Home", routeValues: new { id = course.Id });
        }

        #endregion


        #region CourseQuestion

        public async Task<IActionResult> Question()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> QuestionCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuestionCreate(CourseQuestionViewModel courseQuestionViewModel)
        {
            if (_sharedIdentityService == null || _sharedIdentityService.GetUserId == null)
                return RedirectToAction("Detail", "Home", routeValues: new { id = courseQuestionViewModel.CourseId });


            var user = await _identityService.GetUserProfileAsync(_sharedIdentityService.GetUserId);
            if (user != null || user.Data != null)
            {
                var usr = new CourseUserViewModel
                {
                    Id = user.Data.Id,
                    FirstName = user.Data.FirstName,
                    LastName = user.Data.LastName,
                    Email = user.Data.Email,
                    PhoneNumber = user.Data.PhoneNumber,
                    City = user.Data.City,
                    ProfilePicture = user.Data.ProfilePicture,
                    UserName = user.Data.UserName
                };
                courseQuestionViewModel.User = usr;
                courseQuestionViewModel.CreatedTime = DateTime.Now;


                var result = await _catalogService.CreateCourseQuestion(courseQuestionViewModel);


                return RedirectToAction("Detail", "Home", routeValues: new { id = courseQuestionViewModel.CourseId });
            }

            //var result = await _catalogService.GetByCourseId(courseId);
            return RedirectToAction("Detail", "Home", routeValues: new { id = courseQuestionViewModel.CourseId });
        }

        [HttpPost]
        public async Task<IActionResult> QuestionAnswerCreate(CourseAnswerViewModel courseAnswerViewModel)
        {
            if (_sharedIdentityService == null || _sharedIdentityService.GetUserId == null)
                return RedirectToAction("Detail", "Home", routeValues: new { id = courseAnswerViewModel.CourseId });


            var user = await _identityService.GetUserProfileAsync(_sharedIdentityService.GetUserId);
            if (user != null || user.Data != null)
            {
                var question = await _catalogService.GetQuestionsByCourseIdAsync(courseAnswerViewModel.CourseId);
                if (question != null && question.Data != null)
                {
                    var questionSearched = question.Data.Where(x => x.Id == courseAnswerViewModel.QuestionId).FirstOrDefault();


                    if (questionSearched != null)
                    {
                        var usr = new CourseUserViewModel
                        {
                            Id = user.Data.Id,
                            FirstName = user.Data.FirstName,
                            LastName = user.Data.LastName,
                            Email = user.Data.Email,
                            PhoneNumber = user.Data.PhoneNumber,
                            City = user.Data.City,
                            ProfilePicture = user.Data.ProfilePicture,
                            UserName = user.Data.UserName
                        };
                        courseAnswerViewModel.User = usr;
                        courseAnswerViewModel.CreatedTime = DateTime.Now;
                        questionSearched.Answers.Add(courseAnswerViewModel);
                        var result = await _catalogService.UpdateCourseQuestionAsync(questionSearched);


                        return RedirectToAction("Detail", "Home", routeValues: new { id = courseAnswerViewModel.CourseId });
                    }
                }
            }
            return RedirectToAction("Detail", "Home", routeValues: new { id = courseAnswerViewModel.CourseId });
        }

        #endregion

    }
}
