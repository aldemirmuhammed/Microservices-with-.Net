using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FreeCourse.Web.Models.Catalogs.Course
{
    public class FeatureViewModel
    {
        [Display(Name = "Kurs Süre")]
        public int Duration { get; set; }
    }
}
