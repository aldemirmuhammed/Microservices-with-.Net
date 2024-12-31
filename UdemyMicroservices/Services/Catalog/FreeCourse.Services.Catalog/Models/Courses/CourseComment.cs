using Microsoft.VisualBasic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using FreeCourse.Services.Catalog.Dtos.Courses.CourseUser;
using System.Collections.Generic;

namespace FreeCourse.Services.Catalog.Models.Courses
{
    public class CourseComment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CourseId { get; set; }

        public CourseUserDto User { get; set; }

        public string CommentTitle { get; set; }

        public string CommentDescription { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal CourseRate { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedTime { get; set; }

        public List<CourseUserDto> Like { get; set; }

        public List<CourseUserDto> Dislike { get; set; }

        public bool IsDeleted { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DeletedTime { get; set; }

    }

}
