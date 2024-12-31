using FreeCourse.Services.Catalog.Models.Categories;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace FreeCourse.Services.Catalog.Models.Courses
{
    public class FavoriteCourse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        public string UserId { get; set; }


        public string CourseId { get; set; }


        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedTime { get; set; }
    }
}
