using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models.Categories;
using FreeCourse.Services.Catalog.Models.Courses;
using FreeCourse.Services.Catalog.Services.Interfaces;
using FreeCourse.Services.Catalog.Settings.Database;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Services
{
    public class CourseService : ICourseService
    {


        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;

        private readonly IMapper _mapper;
        private readonly MassTransit.IPublishEndpoint _publishEndpoint;



        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, MassTransit.IPublishEndpoint publishEndpoint)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }


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
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);

        }

        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);
            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);
            if (result == null)
                return Response<NoContent>.Fail("Course not found", 404);


            await _publishEndpoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent
            {
                UserId = courseUpdateDto.UserId,
                CourseId = updateCourse.Id,
                UpdatedName = updateCourse.Name
            });


            return Response<NoContent>.Success(204);
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);
            if (result.DeletedCount > 0)
                return Response<NoContent>.Success(204);
            else
                return Response<NoContent>.Fail("Course not found", 404);
        }



    }
}
