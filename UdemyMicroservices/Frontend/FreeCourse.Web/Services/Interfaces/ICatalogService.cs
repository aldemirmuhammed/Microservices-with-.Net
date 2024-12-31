using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Catalogs;
using FreeCourse.Web.Models.Catalogs.Category;
using FreeCourse.Web.Models.Catalogs.Course.CourseComment;
using FreeCourse.Web.Models.Catalogs.Course.CourseQuestion;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<List<CourseViewModel>> GetAllCourseAsync();
        Task<List<CategoryViewModel>> GetAllCategoryAsync();
        Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId);
        Task<CourseViewModel> GetByCourseId(string courseId);
        Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput);
        Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput);
        Task<bool> DeleteCourseAsync(string courseId);


        #region CourseFavorite

        Task<List<CourseViewModel>> GetAllFavoriteCourseByUserIdAsync(string userId);
        Task<List<FavoriteCourseViewModel>> GetFavoriteCoursesAsync(string userId);
        Task<bool> CreateFavoriteCourseAsync(FavoriteCourseViewModel favoriteCourseViewModel);
        Task<bool> DeleteFavoriteCourseAsync(DeleteFavoriteCourseViewModel deleteFavoriteCourseViewModel);

        #endregion

        #region CourseComment

        Task<Response<List<CourseCommentViewModel>>> GetAllCommentByUserIdAsync(string userId);
        Task<Response<List<CourseCommentViewModel>>> GetAllCommentByCourseIdAsync(string courseId);
        Task<Response<List<CourseCommentViewModel>>> GetAllCommentByUserIdAndCourseIdAsync(CommentByUserIdAndCourseIdViewModel commentByUserIdAndCourseIdViewModel);
        Task<Response<CourseCommentViewModel>> CreateCommentCourseAsync(CourseCommentViewModel courseCommentViewModel);
        Task<Response<bool>> DeleteCommentCourseAsync(string commentId);
        Task<Response<bool>> LikeCommentCourseAsync(string commentId);
        Task<Response<bool>> DislikeCommentCourseAsync(string commentId);

        #endregion


        #region CourseQuestion

        Task<Response<List<CourseQuestionViewModel>>> GetQuestionsByCourseIdAsync(string courseId);

        Task<Response<CourseQuestionViewModel>> CreateCourseQuestion(CourseQuestionViewModel CourseQuestionViewModel);

        Task<Response<bool>> DeleteCourseQuestionAsync(string questionId);

        Task<Response<CourseQuestionViewModel>> UpdateCourseQuestionAsync(CourseQuestionViewModel courseQuestionViewModel);
        #endregion
    }
}
