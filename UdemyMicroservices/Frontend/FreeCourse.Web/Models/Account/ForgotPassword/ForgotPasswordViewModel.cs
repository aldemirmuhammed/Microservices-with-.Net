using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Account.ForgotPassword
{
    public class ForgotPasswordViewModel : AccountBaseViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Code { get; set; } = null;

    }
}
