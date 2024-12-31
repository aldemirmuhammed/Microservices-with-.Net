using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Account.SendEmailConfirmation
{
    public class SendEmailConfirmationViewModel : AccountBaseViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Code { get; set; } = null;

    }
}
