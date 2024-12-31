using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.Catalogs;
using FreeCourse.Web.Models.Catalogs.Category;
using FreeCourse.Web.Models.Catalogs.Course.CourseComment;
using FreeCourse.Web.Models.Catalogs.Course.CourseQuestion;
using FreeCourse.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IPhotoStockService _photoStockService;
        private readonly PhotoHelper _photoHelper;

        public CatalogService(HttpClient httpClient, IPhotoStockService photoStockService, PhotoHelper photoHelper)
        {
            _httpClient = httpClient;
            _photoStockService = photoStockService;
            _photoHelper = photoHelper;
        }

        #region Course

        public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
        {
            var response = await _httpClient.GetAsync("categories");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }


            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CategoryViewModel>>>();
            return responseSuccess.Data;
        }
        public async Task<List<CourseViewModel>> GetAllCourseAsync()
        {
            var response = await _httpClient.GetAsync("courses");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

            responseSuccess.Data.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);

            });

            return responseSuccess.Data;
        }
        public async Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"courses/GetAllByUserId/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var isFavoriteCourse = await GetFavoriteCoursesAsync(userId);

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();
            responseSuccess.Data.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
                if (isFavoriteCourse != null && isFavoriteCourse.Count != 0)
                {
                    if (isFavoriteCourse.Where(y => y.CourseId == x.Id).FirstOrDefault() != null)
                        x.IsFavorite = true;
                }
                //x.Picture = _photoHelper.GetPhotoStockUrl(x.Picture);
            });

            return responseSuccess.Data;
        }
        public async Task<CourseViewModel> GetByCourseId(string courseId)
        {
            var response = await _httpClient.GetAsync($"courses/{courseId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<CourseViewModel>>();
            responseSuccess.Data.StockPictureUrl = _photoHelper.GetPhotoStockUrl(responseSuccess.Data.Picture);
            return responseSuccess.Data;
        }
        public async Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput)
        {

            var resultPhotoService = await _photoStockService.UploadPhoto(courseCreateInput.PhotoFormFile);

            if (resultPhotoService != null)
            {
                courseCreateInput.Picture = resultPhotoService.Url;
            }


            var response = await _httpClient.PostAsJsonAsync<CourseCreateInput>("courses", courseCreateInput);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteCourseAsync(string courseId)
        {
            var response = await _httpClient.DeleteAsync($"courses/{courseId}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput)
        {

            var resultPhotoService = await _photoStockService.UploadPhoto(courseUpdateInput.PhotoFormFile);

            if (resultPhotoService != null)
            {
                await _photoStockService.DeletePhoto(courseUpdateInput.Picture);
                courseUpdateInput.Picture = resultPhotoService.Url;
            }

            var response = await _httpClient.PutAsJsonAsync<CourseUpdateInput>("courses", courseUpdateInput);
            return response.IsSuccessStatusCode;
        }

        #endregion

        #region CourseFavorite

        public async Task<List<CourseViewModel>> GetAllFavoriteCourseByUserIdAsync(string userId)
        {
            var allCourseByUserId = await GetAllCourseAsync();
            var allFavoriteCourseByUserId = await GetFavoriteCoursesAsync(userId);

            if (allFavoriteCourseByUserId == null || allFavoriteCourseByUserId.Count == 0)
                return new List<CourseViewModel>();

            if (allCourseByUserId == null || allCourseByUserId.Count == 0)
                return new List<CourseViewModel>();

            var newFavoriteCourseList = new List<CourseViewModel>();

            allCourseByUserId.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);

                var isExistFavoriteItem = allFavoriteCourseByUserId.Where(y => y.CourseId == x.Id).FirstOrDefault();
                if (isExistFavoriteItem != null)
                {
                    x.IsFavorite = true;
                    newFavoriteCourseList.Add(x);
                }
            });

            return newFavoriteCourseList;
        }

        public async Task<List<FavoriteCourseViewModel>> GetFavoriteCoursesAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"courses/GetFavoriteCourses/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<FavoriteCourseViewModel>>>();
            return responseSuccess.Data;
        }

        public async Task<bool> CreateFavoriteCourseAsync(FavoriteCourseViewModel favoriteCourseViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync<FavoriteCourseViewModel>("courses/CreateFavoriteCourse", favoriteCourseViewModel);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteFavoriteCourseAsync(DeleteFavoriteCourseViewModel deleteFavoriteCourseViewModel)
        {
            var response = await _httpClient.DeleteAsync($"courses/DeleteFavoriteCourse/{JsonSerializer.Serialize(deleteFavoriteCourseViewModel)}");
            return response.IsSuccessStatusCode;
        }

        #endregion

        #region CourseComment

        public async Task<Response<List<CourseCommentViewModel>>> GetAllCommentByUserIdAsync(string userId)
        {
            if (userId == null)
                return Response<List<CourseCommentViewModel>>.Fail("An error occurred while getting comments.", 404);

            var response = await _httpClient.GetAsync($"courses/GetAllCommentByUserId/{userId}");
            if (!response.IsSuccessStatusCode)
                return Response<List<CourseCommentViewModel>>.Fail("An error occurred while getting comments.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<List<CourseCommentViewModel>>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<List<CourseCommentViewModel>>.Fail(result.Erros, 404);

            return Response<List<CourseCommentViewModel>>.Success(result.Data, 200);
        }

        public async Task<Response<List<CourseCommentViewModel>>> GetAllCommentByCourseIdAsync(string courseId)
        {
            if (courseId == null)
                return Response<List<CourseCommentViewModel>>.Fail("An error occurred while getting comments.", 404);

            var response = await _httpClient.GetAsync($"courses/get-all-comment-by-course-id/{courseId}");
            if (!response.IsSuccessStatusCode)
                return Response<List<CourseCommentViewModel>>.Fail("An error occurred while getting comments.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<List<CourseCommentViewModel>>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<List<CourseCommentViewModel>>.Fail(result.Erros, 404);

            return Response<List<CourseCommentViewModel>>.Success(result.Data, 200);
        }

        public async Task<Response<List<CourseCommentViewModel>>> GetAllCommentByUserIdAndCourseIdAsync(CommentByUserIdAndCourseIdViewModel commentByUserIdAndCourseIdViewModel)
        {
            if (commentByUserIdAndCourseIdViewModel == null)
                return Response<List<CourseCommentViewModel>>.Fail("An error occurred while getting comments.", 404);

            var response = await _httpClient.PostAsJsonAsync($"courses/GetAllCommentByUserIdAndCourseId", commentByUserIdAndCourseIdViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<List<CourseCommentViewModel>>.Fail("An error occurred while getting comments.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<List<CourseCommentViewModel>>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<List<CourseCommentViewModel>>.Fail(result.Erros, 404);

            return Response<List<CourseCommentViewModel>>.Success(result.Data, 200);
        }

        public async Task<Response<CourseCommentViewModel>> CreateCommentCourseAsync(CourseCommentViewModel courseCommentViewModel)
        {
            if (courseCommentViewModel == null)
                return Response<CourseCommentViewModel>.Fail("An error occurred while creating comments.", 404);

            var response = await _httpClient.PostAsJsonAsync($"courses/CreateCommentCourse", courseCommentViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<CourseCommentViewModel>.Fail("An error occurred while creating comments.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<CourseCommentViewModel>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<CourseCommentViewModel>.Fail(result.Erros, 404);

            return Response<CourseCommentViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<bool>> DeleteCommentCourseAsync(string commentId)
        {
            if (commentId == null)
                return Response<bool>.Fail("An error occurred while deleting comments.", 404);

            var response = await _httpClient.DeleteAsync($"courses/DeleteCommentCourse/{commentId}");
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("An error occurred while deleting comments.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            if (!result.Data || (result.Erros != null && result.Erros.Any()))
                return Response<bool>.Fail(result.Erros, 404);

            return Response<bool>.Success(result.Data, 200);
        }

        public async Task<Response<bool>> LikeCommentCourseAsync(string commentId)
        {
            if (commentId == null)
                return Response<bool>.Fail("An error occurred while deleting comments.", 404);

            var response = await _httpClient.GetAsync($"courses/LikeCommentCourse/{commentId}");
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("An error occurred while deleting comments.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            if (!result.Data || (result.Erros != null && result.Erros.Any()))
                return Response<bool>.Fail(result.Erros, 404);

            return Response<bool>.Success(result.Data, 200);
        }

        public async Task<Response<bool>> DislikeCommentCourseAsync(string commentId)
        {
            if (commentId == null)
                return Response<bool>.Fail("An error occurred while deleting comments.", 404);

            var response = await _httpClient.GetAsync($"courses/DislikeCommentCourse/{commentId}");
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("An error occurred while deleting comments.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            if (!result.Data || (result.Erros != null && result.Erros.Any()))
                return Response<bool>.Fail(result.Erros, 404);

            return Response<bool>.Success(result.Data, 200);
        }


        #endregion


        #region CourseQuestion

        public async Task<Response<List<CourseQuestionViewModel>>> GetQuestionsByCourseIdAsync(string courseId)
        {
            if (courseId == null)
                return Response<List<CourseQuestionViewModel>>.Fail("An error occurred while getting questions.", 404);

            var response = await _httpClient.GetAsync($"courses/GetQuestionsByCourseId/{courseId}");
            if (!response.IsSuccessStatusCode)
                return Response<List<CourseQuestionViewModel>>.Fail("An error occurred while getting questions.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<List<CourseQuestionViewModel>>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<List<CourseQuestionViewModel>>.Fail(result.Erros, 404);

            return Response<List<CourseQuestionViewModel>>.Success(result.Data, 200);
        }

        public async Task<Response<CourseQuestionViewModel>> CreateCourseQuestion(CourseQuestionViewModel CourseQuestionViewModel)
        {
            if (CourseQuestionViewModel == null)
                return Response<CourseQuestionViewModel>.Fail("An error occurred while getting questions.", 404);

            var response = await _httpClient.PostAsJsonAsync($"courses/create-course-question", CourseQuestionViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<CourseQuestionViewModel>.Fail("An error occurred while getting questions.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<CourseQuestionViewModel>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<CourseQuestionViewModel>.Fail(result.Erros, 404);

            return Response<CourseQuestionViewModel>.Success(result.Data, 200);
        }

        public async Task<Response<bool>> DeleteCourseQuestionAsync(string questionId)
        {
            if (questionId == null)
                return Response<bool>.Fail("An error occurred while getting questions.", 404);

            var response = await _httpClient.DeleteAsync($"courses/DeleteCourseQuestion/{questionId}");
            if (!response.IsSuccessStatusCode)
                return Response<bool>.Fail("An error occurred while getting questions.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<bool>>();
            if (!result.Data || (result.Erros != null && result.Erros.Any()))
                return Response<bool>.Fail(result.Erros, 404);

            return Response<bool>.Success(result.Data, 200);
        }

        public async Task<Response<CourseQuestionViewModel>> UpdateCourseQuestionAsync(CourseQuestionViewModel courseQuestionViewModel)
        {
            if (courseQuestionViewModel == null)
                return Response<CourseQuestionViewModel>.Fail("An error occurred while creating questions.", 404);

            var response = await _httpClient.PostAsJsonAsync($"courses/UpdateCourseQuestion", courseQuestionViewModel);
            if (!response.IsSuccessStatusCode)
                return Response<CourseQuestionViewModel>.Fail("An error occurred while creating questions.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<CourseQuestionViewModel>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<CourseQuestionViewModel>.Fail(result.Erros, 404);

            return Response<CourseQuestionViewModel>.Success(result.Data, 200);
        }

  

        #endregion

    }
}
