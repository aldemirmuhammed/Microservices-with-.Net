using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Account.EnableAuthenticator
{
    public class EnableAuthenticatorViewModel : AccountBaseViewModel
    {
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        public string[] RecoveryCodes { get; set; }
    }
}
