using FreeCourse.Services.Catalog.Dtos.Courses;
using FreeCourse.Services.Catalog.Dtos.Courses.CommentCourses;
using FreeCourse.Services.Catalog.Dtos.Courses.CourseQuestion;
using FreeCourse.Services.Catalog.Dtos.Courses.FavoriteCourses;
using FreeCourse.Services.Catalog.Models.Courses;
using FreeCourse.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Services.Interfaces
{
    public interface ICourseService
    {
        Task<Response<List<CourseDto>>> GetAllAsync();

        Task<Response<CourseDto>> GetByIdAsync(string id);

        Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId);

        Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto);

        Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto);

        Task<Response<NoContent>> DeleteAsync(string id);

        #region CourseFavorite

        Task<Response<List<FavoriteCourse>>> GetAllFavoriteAsync(string userId);
        Task<Response<NoContent>> DeleteFavoriteCourseAsync(DeleteFavoriteCourseDto deleteFavoriteCourseDto);
        Task<Response<FavoriteCourse>> CreateFavoriteCourseAsync(FavoriteCourse favoriteCourse);

        #endregion

        #region CoursesComment

        Task<Response<List<CourseCommentDto>>> GetAllCommentByUserIdAsync(string userId);

        Task<Response<List<CourseCommentDto>>> GetAllCommentByCourseIdAsync(string courseId);

        Task<Response<List<CourseCommentDto>>> GetAllCommentByUserIdAndCourseIdAsync(CommentByUserIdAndCourseId commentByUserIdAndCourseId);

        Task<Response<CourseCommentDto>> CreateCommentCourseAsync(CourseCommentDto courseComment);

        Task<Response<bool>> DeleteCommentCourseAsync(string commentId);

        Task<Response<bool>> LikeCommentCourseAsync(string commentId);

        Task<Response<bool>> DislikeCommentCourseAsync(string commentId);

        #endregion

        #region CourseQuestion

        Task<Response<List<CourseQuestionDto>>> GetQuestionsByCourseIdAsync(string courseId);

        Task<Response<CourseQuestionDto>> CreateCourseQuestionAsync(CourseQuestionDto courseQuestionDto);

        Task<Response<bool>> DeleteCourseQuestionAsync(string questionId);

        Task<Response<CourseQuestionDto>> UpdateCourseQuestionAsync(CourseQuestionDto courseQuestionDto);


        #endregion
    }
}
