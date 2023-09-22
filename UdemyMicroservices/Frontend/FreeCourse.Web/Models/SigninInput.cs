using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models
{
    public class SigninInput
    {
        [Required]
        [Display(Name="Email adresiniz")]
        public string Email{ get; set; }

        [Required]
        [Display(Name = "Email şifreniz")]
        public string Password{ get; set; }

        [Display(Name="Beni hatırla")]
        public bool IsRemember{ get; set; }
    }
}
