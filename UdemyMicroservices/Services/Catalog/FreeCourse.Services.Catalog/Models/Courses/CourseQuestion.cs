using Microsoft.VisualBasic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using FreeCourse.Services.Catalog.Dtos.Courses.CourseUser;

namespace FreeCourse.Services.Catalog.Models.Courses
{
    public class CourseQuestion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CourseId { get; set; }

        public string QuestionTitle { get; set; }

        public string QuestionDescription { get; set; }

        public List<CourseAnswer> Answers { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedTime { get; set; }

        public bool IsDeleted { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DeletedTime { get; set; }

        public CourseUserDto User { get; set; }


    }

    public class CourseAnswer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string QuestionId { get; set; }

        public string CourseId { get; set; }

        public string AnswerDescription { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedTime { get; set; }

        public List<CourseUserDto> AsnwerLike { get; set; }

        public List<CourseUserDto> AsnwerDislike { get; set; }

        public bool IsDeleted { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DeletedTime { get; set; }

        public CourseUserDto User { get; set; }

    }

}
