using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using FreeCourse.Web.Models.Catalogs.Category;
using FreeCourse.Web.Models.Catalogs.Course;
using FreeCourse.Web.Models.Catalogs.Course.CourseComment;
using FreeCourse.Web.Models.Catalogs.Course.CourseQuestion;

namespace FreeCourse.Web.Models.Catalogs
{
    public class CourseViewModel
    {


        public string Id { get; set; }
        public string UserId { get; set; }
        public string CategoryId { get; set; }


        public string Name { get; set; }


        public decimal Price { get; set; }


        public string Description { get; set; }

        public string ShortDescription
        {
            get => (Description != null && Description.Length > 100) ? Description.Substring(0, 100) + "..." : Description;
        }


        public string Picture { get; set; }

        public string StockPictureUrl { get; set; }

        public DateTime CreatedTime { get; set; }

        public FeatureViewModel Feature { get; set; }


        public CategoryViewModel Category { get; set; }

        public decimal CourseRate { get; set; }

        [JsonIgnore]
        public bool IsFavorite { get; set; } = false;


        [JsonIgnore]
        public List<CourseCommentViewModel> CourseCommentList { get; set; }


        [JsonIgnore]
        public List<CourseQuestionViewModel> CourseQuestionsList { get; set; }

    }
}
