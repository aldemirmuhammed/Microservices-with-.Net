
using FreeCourse.Services.Catalog.Dtos.Courses.CourseUser;
using System;
using System.Collections.Generic;

namespace FreeCourse.Services.Catalog.Dtos.Courses.CommentCourses
{
    public class CourseCommentDto
    {
        public string Id { get; set; }

        public string CourseId { get; set; }

        public CourseUserDto User { get; set; }

        public string CommentTitle { get; set; }

        public string CommentDescription { get; set; }

        public decimal CourseRate { get; set; }

        public DateTime CreatedTime { get; set; }

        public List<CourseUserDto> Like { get; set; }

        public List<CourseUserDto> Dislike { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateTime {  get; set; }

    }

}
