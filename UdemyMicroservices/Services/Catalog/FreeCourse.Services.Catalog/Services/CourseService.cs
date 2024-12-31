using AutoMapper;
using FreeCourse.Services.Catalog.Dtos.Courses;
using FreeCourse.Services.Catalog.Dtos.Courses.CommentCourses;
using FreeCourse.Services.Catalog.Dtos.Courses.CourseQuestion;
using FreeCourse.Services.Catalog.Dtos.Courses.FavoriteCourses;
using FreeCourse.Services.Catalog.Models.Categories;
using FreeCourse.Services.Catalog.Models.Courses;
using FreeCourse.Services.Catalog.Services.Interfaces;
using FreeCourse.Services.Catalog.Settings.Database;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Services
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMongoCollection<FavoriteCourse> _favoriteCourseCollection;
        private readonly IMongoCollection<CourseComment> _coursesCommentCollection;
        private readonly IMongoCollection<CourseQuestion> _coursesQuestionsCollection;

        private readonly IMapper _mapper;
        private readonly IMessageQueueService _messageQueueService;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, IMessageQueueService messageQueueService)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _favoriteCourseCollection = database.GetCollection<FavoriteCourse>(databaseSettings.FavoriteCourseCollectionName);
            _coursesCommentCollection = database.GetCollection<CourseComment>(databaseSettings.CourseCommentCollectionName);
            _coursesQuestionsCollection = database.GetCollection<CourseQuestion>(databaseSettings.CourseQuestionsCollectionName);

            _mapper = mapper;
            _messageQueueService = messageQueueService;
        }

        #region Course

        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(course => true).ToListAsync();
            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }
            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var courses = await _courseCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (courses == null)
            {
                return Response<CourseDto>.Fail("Course not found", 404);
            }
            courses.Category = await _categoryCollection.Find(x => x.Id == courses.CategoryId).FirstAsync();
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(courses), 200);
        }

        public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();
            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {

            var newCourse = _mapper.Map<Course>(courseCreateDto);
            newCourse.CreatedTime = DateTime.Now;
            await _courseCollection.InsertOneAsync(newCourse);

            await _messageQueueService.SendToNotification(courseCreateDto.UserId, "Course create", "Course create operation successfully completed.");

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);

        }

        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);
            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);
            if (result == null)
                return Response<NoContent>.Fail("Course not found", 404);

            await _messageQueueService.SendToBasket(courseUpdateDto.UserId, updateCourse.Id, updateCourse.Name);

            await _messageQueueService.SendToNotification(courseUpdateDto.UserId, "Course updated successfully", "Course updated oeration successfully completed.");


            return Response<NoContent>.Success(204);
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);
            var favoriteCourseResult = await _favoriteCourseCollection.DeleteOneAsync(x => x.CourseId == id);


            await _messageQueueService.SendToNotification(id, "Course delete", "Course delete operation successfully completed.");


            if (result.DeletedCount > 0)
                return Response<NoContent>.Success(204);
            else
                return Response<NoContent>.Fail("Course not found", 404);
        }

        #endregion

        #region CoursesFavorite

        public async Task<Response<List<FavoriteCourse>>> GetAllFavoriteAsync(string userId)
        {
            var favoriteCourses = await _favoriteCourseCollection.Find(x => x.UserId == userId).ToListAsync();
            if (favoriteCourses.Any())
            {
                return Response<List<FavoriteCourse>>.Success(favoriteCourses, 200);

            }
            return Response<List<FavoriteCourse>>.Fail("Favorite course could not found", 404);

        }
        public async Task<Response<FavoriteCourse>> CreateFavoriteCourseAsync(FavoriteCourse favoriteCourse)
        {
            var favoriteList = await GetAllFavoriteAsync(favoriteCourse.UserId);
            if (favoriteList != null && favoriteList.Data != null)
            {
                var isExist = favoriteList.Data.Any(x => x.CourseId == favoriteCourse.CourseId);
                if (isExist)
                {
                    return Response<FavoriteCourse>.Fail("Course already add favorite list", 404);
                }

            }

            favoriteCourse.CreatedTime = DateTime.Now;
            await _favoriteCourseCollection.InsertOneAsync(favoriteCourse);
            return Response<FavoriteCourse>.Success(favoriteCourse, 200);

        }
        public async Task<Response<NoContent>> DeleteFavoriteCourseAsync(DeleteFavoriteCourseDto deleteFavoriteCourseDto)
        {
            var result = await _favoriteCourseCollection.DeleteOneAsync(x => x.UserId == deleteFavoriteCourseDto.UserId && x.CourseId == deleteFavoriteCourseDto.CourseId);

            if (result.DeletedCount > 0)
                return Response<NoContent>.Success(204);
            else
                return Response<NoContent>.Fail("FAvorite course not found", 404);
        }

        #endregion

        #region CoursesComment

        public async Task<Response<List<CourseCommentDto>>> GetAllCommentByUserIdAsync(string userId)
        {
            if (userId == null)
                return Response<List<CourseCommentDto>>.Fail("Course comment could not found", 404);
            var coursesComment = await _coursesCommentCollection.Find(x => x.User.Id == userId && x.IsDeleted == false).ToListAsync();
            if (coursesComment.Any())
                return Response<List<CourseCommentDto>>.Success(_mapper.Map<List<CourseCommentDto>>(coursesComment), 200);
            return Response<List<CourseCommentDto>>.Fail("Course comment could not found by user id error", 404);
        }

        public async Task<Response<List<CourseCommentDto>>> GetAllCommentByCourseIdAsync(string courseId)
        {
            if (courseId == null)
                return Response<List<CourseCommentDto>>.Fail("Course comment could not found", 404);
            var coursesComment = await _coursesCommentCollection.Find(x => x.CourseId == courseId && x.IsDeleted == false).ToListAsync();
            return Response<List<CourseCommentDto>>.Success(_mapper.Map<List<CourseCommentDto>>(coursesComment), 200);

        }

        public async Task<Response<List<CourseCommentDto>>> GetAllCommentByUserIdAndCourseIdAsync(CommentByUserIdAndCourseId commentByUserIdAndCourseId)
        {
            if (commentByUserIdAndCourseId == null || commentByUserIdAndCourseId.UserId == null)
                return Response<List<CourseCommentDto>>.Fail("Course comment could not found", 404);

            var coursesComment = await _coursesCommentCollection.Find(x => x.User.Id == commentByUserIdAndCourseId.UserId
            && x.CourseId == commentByUserIdAndCourseId.CourseId
            && x.IsDeleted == false).ToListAsync();
            if (coursesComment.Any())
                return Response<List<CourseCommentDto>>.Success(_mapper.Map<List<CourseCommentDto>>(coursesComment), 200);
            return Response<List<CourseCommentDto>>.Fail("Comment could not found by course id and user id  error", 404);
        }

        public async Task<Response<CourseCommentDto>> CreateCommentCourseAsync(CourseCommentDto courseComment)
        {
            if (courseComment == null || courseComment.User.Id == null)
                return Response<CourseCommentDto>.Fail("Course comment could not found", 404);


            courseComment.CreatedTime = DateTime.Now;
            await _coursesCommentCollection.InsertOneAsync(_mapper.Map<CourseComment>(courseComment));

            var course = await _courseCollection.Find(x => x.Id == courseComment.CourseId).FirstOrDefaultAsync();
            if (course == null && courseComment.User.Id != null)
                course = await _courseCollection.Find(x => x.UserId == courseComment.User.Id).FirstOrDefaultAsync();

            if (course != null)
            {
                var courseCommentList = await GetAllCommentByCourseIdAsync(course.Id);
                if (courseCommentList != null && courseCommentList.Data != null)
                {
                    decimal _commentCount = 0;
                    foreach (var item in courseCommentList.Data)
                        _commentCount += item.CourseRate;

                    course.CourseRate = _commentCount / courseCommentList.Data.Count;
                    await _courseCollection.FindOneAndReplaceAsync(x => x.Id == course.Id, course);

                }
            }
            return Response<CourseCommentDto>.Success(courseComment, 200);
        }

        public async Task<Response<bool>> DeleteCommentCourseAsync(string commentId)
        {
            if (commentId == null)
                return Response<bool>.Fail("Course comment could not found", 404);

            var result = await _coursesCommentCollection.Find(x => x.Id == commentId).ToListAsync();
            if (result == null)
                return Response<bool>.Fail("Course comment could not found", 404);

            foreach (var item in result)
            {
                item.IsDeleted = true;
                item.DeletedTime = DateTime.Now;
                var aa = await _coursesCommentCollection.FindOneAndReplaceAsync(x => x.Id == item.Id, item);
            }
            return Response<bool>.Success(true, 204);
        }

        public async Task<Response<bool>> LikeCommentCourseAsync(string commentId)
        {
            if (commentId == null)
                return Response<bool>.Fail("Course comment could not found", 404);


            var coursesComment = await _coursesCommentCollection.Find(x => x.Id == commentId).ToListAsync();
            if (coursesComment.Any())
            {
                foreach (var item in coursesComment)
                {
                    var isExist = item.Like.Where(x => x.Id == item.User.Id).FirstOrDefault();
                    if (isExist == null)
                    {
                        item.Like.Add(item.User);
                        await _coursesCommentCollection.FindOneAndReplaceAsync(x => x.Id == item.Id, item);
                    }
                }
                return Response<bool>.Success(true, 200);
            }
            return Response<bool>.Fail("Course comment could not like error", 404);
        }

        public async Task<Response<bool>> DislikeCommentCourseAsync(string commentId)
        {
            if (commentId == null)
                return Response<bool>.Fail("Course comment could not found", 404);

            var coursesComment = await _coursesCommentCollection.Find(x => x.Id == commentId).ToListAsync();
            if (coursesComment.Any())
            {
                foreach (var item in coursesComment)
                {
                    var isExist = item.Dislike.Where(x => x.Id == item.User.Id).FirstOrDefault();
                    if (isExist == null)
                    {
                        item.Dislike.Add(item.User);
                        await _coursesCommentCollection.FindOneAndReplaceAsync(x => x.Id == item.Id, item);
                    }
                }
                return Response<bool>.Success(true, 200);
            }
            return Response<bool>.Fail("Course comment could not dislike error", 404);
        }

        #endregion

        #region CourseQuestions

        public async Task<Response<List<CourseQuestionDto>>> GetQuestionsByCourseIdAsync(string courseId)
        {
            if (courseId == null)
                return Response<List<CourseQuestionDto>>.Fail("Course question could not found", 404);

            var coursesQuestion = await _coursesQuestionsCollection.Find(x => x.CourseId == courseId && x.IsDeleted == false).ToListAsync();
            if (coursesQuestion.Any())
                return Response<List<CourseQuestionDto>>.Success(_mapper.Map<List<CourseQuestionDto>>(coursesQuestion), 200);

            return Response<List<CourseQuestionDto>>.Fail("Course question could not found by user id error", 404);
        }

        public async Task<Response<CourseQuestionDto>> CreateCourseQuestionAsync(CourseQuestionDto courseQuestionDto)
        {
            if (courseQuestionDto == null || courseQuestionDto.User.Id == null)
                return Response<CourseQuestionDto>.Fail("Course question could not found", 404);


            courseQuestionDto.CreatedTime = DateTime.Now;
            await _coursesQuestionsCollection.InsertOneAsync(_mapper.Map<CourseQuestion>(courseQuestionDto));
            return Response<CourseQuestionDto>.Success(courseQuestionDto, 200);
        }

        public async Task<Response<bool>> DeleteCourseQuestionAsync(string questionId)
        {
            if (questionId == null)
                return Response<bool>.Fail("Course question could not found", 404);

            var result = await _coursesQuestionsCollection.Find(x => x.Id == questionId).ToListAsync();
            if (result == null)
                return Response<bool>.Fail("Course question could not found", 404);

            foreach (var item in result)
            {
                item.IsDeleted = true;
                item.DeletedTime = DateTime.Now;
                var aa = await _coursesQuestionsCollection.FindOneAndReplaceAsync(x => x.Id == item.Id, item);
            }
            return Response<bool>.Success(true, 204);
        }

        public async Task<Response<CourseQuestionDto>> UpdateCourseQuestionAsync(CourseQuestionDto courseQuestionDto)
        {
            if (courseQuestionDto == null)
                return Response<CourseQuestionDto>.Fail("Course question could not found", 404);

            //var result = await _coursesQuestionsCollection.Find(x => x.Id == courseQuestionDto.Id).FirstOrDefaultAsync();
            //if (result == null)
            //    return Response<CourseQuestionDto>.Fail("Course question could not found", 404);

            await _coursesQuestionsCollection.FindOneAndReplaceAsync(x => x.Id == courseQuestionDto.Id, _mapper.Map<CourseQuestion>(courseQuestionDto));
            return Response<CourseQuestionDto>.Success(courseQuestionDto, 200);
        }

        #endregion


    }
}
