using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;
using System;
using FreeCourse.Services.Catalog.Dtos.Courses.CourseUser;

namespace FreeCourse.Services.Catalog.Dtos.Courses.CourseQuestion
{
    public class CourseQuestionDto
    {
        public string Id { get; set; }

        public string CourseId { get; set; }

        public string QuestionTitle { get; set; }

        public string QuestionDescription { get; set; }

        public List<CourseAnswerDto> Answers { get; set; }

        public DateTime CreatedTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DeletedTime { get; set; }

        public CourseUserDto User { get; set; }

    }

    public class CourseAnswerDto
    {
       
        public string Id { get; set; }

        public string QuestionId { get; set; }

        public string CourseId { get; set; }

        public string AnswerDescription { get; set; }

        public DateTime CreatedTime { get; set; }

        public List<CourseUserDto> AsnwerLike { get; set; }

        public List<CourseUserDto> AsnwerDislike { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DeletedTime { get; set; }

        public CourseUserDto User { get; set; }

    }
}
