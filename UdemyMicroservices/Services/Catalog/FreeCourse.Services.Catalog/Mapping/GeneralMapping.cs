using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Dtos.Courses;
using FreeCourse.Services.Catalog.Dtos.Courses.CommentCourses;
using FreeCourse.Services.Catalog.Dtos.Courses.CourseQuestion;
using FreeCourse.Services.Catalog.Models.Categories;
using FreeCourse.Services.Catalog.Models.Courses;
using FreeCourse.Services.Catalog.Models.Features;

namespace FreeCourse.Services.Catalog.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Feature, FeatureDto>().ReverseMap();


            CreateMap<Course, CourseCreateDto>().ReverseMap();
            CreateMap<Course, CourseUpdateDto>().ReverseMap();


            CreateMap<CourseComment, CourseCommentDto>().ReverseMap();
            CreateMap<CourseQuestion, CourseQuestionDto>().ReverseMap();
            CreateMap<CourseAnswer, CourseAnswerDto>().ReverseMap();
        }
    }
}
