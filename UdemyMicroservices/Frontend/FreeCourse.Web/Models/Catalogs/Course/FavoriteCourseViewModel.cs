using System;

namespace FreeCourse.Web.Models.Catalogs
{
    public class FavoriteCourseViewModel
    {
        public string Id { get; set; }


        public string UserId { get; set; }


        public string CourseId { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
