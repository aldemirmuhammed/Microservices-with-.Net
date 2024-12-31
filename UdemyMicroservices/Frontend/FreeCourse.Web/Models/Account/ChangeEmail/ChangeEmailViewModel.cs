using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FreeCourse.Web.Models.Account.ChangeEmail
{
    public class ChangeEmailViewModel : AccountBaseViewModel
    {
     
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }

        public string Token { get; set; } = null;


    }
}
