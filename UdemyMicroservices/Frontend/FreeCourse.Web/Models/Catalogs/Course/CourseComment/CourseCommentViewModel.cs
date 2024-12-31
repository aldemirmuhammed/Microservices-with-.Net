
using FreeCourse.Web.Models.Account;
using FreeCourse.Web.Models.Catalogs.Course.CourseUser;
using System;
using System.Collections.Generic;

namespace FreeCourse.Web.Models.Catalogs.Course.CourseComment
{
    public class CourseCommentViewModel
    {
        public string Id { get; set; }

        public string CourseId { get; set; }

        public CourseUserViewModel User { get; set; }

        //public List<CourseUserViewModel> CourseUsers { get; set; }

        public string CommentTitle { get; set; }

        public string CommentDescription { get; set; }

        public decimal CourseRate { get; set; }

        public DateTime CreatedTime { get; set; }

        public List<CourseUserViewModel> Like { get; set; }

        public List<CourseUserViewModel> Dislike { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DeletedTime {  get; set; }

    }

  
}
