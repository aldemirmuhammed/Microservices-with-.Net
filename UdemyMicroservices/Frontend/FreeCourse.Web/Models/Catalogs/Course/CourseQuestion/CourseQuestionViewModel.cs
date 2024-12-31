using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;
using FreeCourse.Web.Models.Account;
using FreeCourse.Web.Models.Catalogs.Course.CourseUser;

namespace FreeCourse.Web.Models.Catalogs.Course.CourseQuestion
{
    public class CourseQuestionViewModel
    {

        public string Id { get; set; }

        public string CourseId { get; set; }

        public string QuestionTitle { get; set; }

        public string QuestionDescription { get; set; }

        public List<CourseAnswerViewModel> Answers { get; set; }

        public DateTime CreatedTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DeletedTime { get; set; }

        public CourseUserViewModel User { get; set; }

    }

    public class CourseAnswerViewModel
    {

        public string Id { get; set; }

        public string QuestionId { get; set; }

        public string CourseId { get; set; }

        public string AnswerDescription { get; set; }

        public DateTime CreatedTime { get; set; }

        public List<CourseUserViewModel> AsnwerLike { get; set; }

        public List<CourseUserViewModel> AsnwerDislike { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DeletedTime { get; set; }

        public CourseUserViewModel User { get; set; }

    }


}
