using FreeCourse.Services.Catalog.Dtos.Courses;
using FreeCourse.Services.Catalog.Dtos.Courses.CommentCourses;
using FreeCourse.Services.Catalog.Dtos.Courses.CourseQuestion;
using FreeCourse.Services.Catalog.Dtos.Courses.FavoriteCourses;
using FreeCourse.Services.Catalog.Models.Courses;
using FreeCourse.Services.Catalog.Services.Interfaces;
using FreeCourse.Shared.ControllerBases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : CustomBaseController
    {
        private readonly ICourseService _courseService;
        public CoursesController(ICourseService courseServices)
        {
            _courseService = courseServices;
        }

        #region Course


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _courseService.GetAllAsync();

            return CreateActionResultInstance(response);

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _courseService.GetByIdAsync(id);

            return CreateActionResultInstance(response);

        }

        [HttpGet]
        [Route("/api/[controller]/GetAllByUserId/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var response = await _courseService.GetAllByUserIdAsync(userId);

            return CreateActionResultInstance(response);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateDto courseCreateDto)
        {
            var response = await _courseService.CreateAsync(courseCreateDto);

            return CreateActionResultInstance(response);

        }


        [HttpPut]
        public async Task<IActionResult> Update(CourseUpdateDto courseUpdateDto)
        {
            var response = await _courseService.UpdateAsync(courseUpdateDto);

            return CreateActionResultInstance(response);

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _courseService.DeleteAsync(id);

            return CreateActionResultInstance(response);

        }


        #endregion

        #region CourseFavorite

        [HttpGet]
        [Route("/api/[controller]/GetFavoriteCourses/{userId}")]
        public async Task<IActionResult> GetFavoriteCourses(string userId)
        {
            var response = await _courseService.GetAllFavoriteAsync(userId);
            return CreateActionResultInstance(response);

        }

        [HttpPost]
        [Route("/api/[controller]/CreateFavoriteCourse")]
        public async Task<IActionResult> CreateFavoriteCourse(FavoriteCourse favoriteCourse)
        {
            var response = await _courseService.CreateFavoriteCourseAsync(favoriteCourse);
            return CreateActionResultInstance(response);

        }

        [HttpDelete("{id}")]
        [Route("/api/[controller]/DeleteFavoriteCourse/{id}")]
        public async Task<IActionResult> DeleteFavoriteCourse(string id)
        {
            var delete = JsonSerializer.Deserialize<DeleteFavoriteCourseDto>(id);
            var response = await _courseService.DeleteFavoriteCourseAsync(delete);
            return CreateActionResultInstance(response);
        }

        #endregion

        #region CourseComment

        [HttpGet]
        [Route("/api/[controller]/GetAllCommentByUserId/{userId}")]
        public async Task<IActionResult> GetAllCommentByUserId(string userId)
        {
            return CreateActionResultInstance(await _courseService.GetAllCommentByUserIdAsync(userId));

        }

        [HttpGet]
        [Route("/api/[controller]/get-all-comment-by-course-id/{courseId}")]
        public async Task<IActionResult> GetAllCommentByCourseId(string courseId)
        {
            return CreateActionResultInstance(await _courseService.GetAllCommentByCourseIdAsync(courseId));

        }

        [HttpPost]
        [Route("/api/[controller]/GetAllCommentByUserIdAndCourseId")]
        public async Task<IActionResult> GetAllCommentByUserIdAndCourseId(CommentByUserIdAndCourseId commentByUserIdAndCourseId)
        {
            return CreateActionResultInstance(await _courseService.GetAllCommentByUserIdAndCourseIdAsync(commentByUserIdAndCourseId));

        }


        [HttpPost]
        [Route("/api/[controller]/CreateCommentCourse")]
        public async Task<IActionResult> CreateCommentCourse(CourseCommentDto courseCommentDto)
        {
            return CreateActionResultInstance(await _courseService.CreateCommentCourseAsync(courseCommentDto));
        }

        [HttpDelete("{commentId}")]
        [Route("/api/[controller]/DeleteCommentCourse/{commentId}")]
        public async Task<IActionResult> DeleteCommentCourse(string commentId)
        {
            return CreateActionResultInstance(await _courseService.DeleteCommentCourseAsync(commentId));
        }

        [HttpGet]
        [Route("/api/[controller]/LikeCommentCourse/{commentId}")]
        public async Task<IActionResult> LikeCommentCourse(string commentId)
        {
            return CreateActionResultInstance(await _courseService.LikeCommentCourseAsync(commentId));
        }

        [HttpGet]
        [Route("/api/[controller]/DislikeCommentCourse/{commentId}")]
        public async Task<IActionResult> DislikeCommentCourse(string commentId)
        {
            return CreateActionResultInstance(await _courseService.DislikeCommentCourseAsync(commentId));
        }

        #endregion

        #region CourseQuestion

        [HttpGet]
        [Route("/api/[controller]/GetQuestionsByCourseId/{courseId}")]
        public async Task<IActionResult> GetQuestionsByCourseId(string courseId)
        {
            return CreateActionResultInstance(await _courseService.GetQuestionsByCourseIdAsync(courseId));

        }

        [HttpPost]
        [Route("/api/[controller]/create-course-question")]
        public async Task<IActionResult> CreateCourseQuestion(CourseQuestionDto courseQuestionDto)
        {
            return CreateActionResultInstance(await _courseService.CreateCourseQuestionAsync(courseQuestionDto));

        }

        [HttpDelete("{questionId}")]
        [Route("/api/[controller]/DeleteCourseQuestion/{questionId}")]
        public async Task<IActionResult> DeleteCourseQuestion(string questionId)
        {
            return CreateActionResultInstance(await _courseService.DeleteCourseQuestionAsync(questionId));

        }


        [HttpPost]
        [Route("/api/[controller]/UpdateCourseQuestion")]
        public async Task<IActionResult> UpdateCourseQuestion(CourseQuestionDto courseQuestionDto)
        {
            return CreateActionResultInstance(await _courseService.UpdateCourseQuestionAsync(courseQuestionDto));
        }

        #endregion

    }
}
