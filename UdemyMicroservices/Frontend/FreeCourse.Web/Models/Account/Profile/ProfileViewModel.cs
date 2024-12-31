using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Account.Profile
{
    public class ProfileViewModel : AccountBaseViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Profile Picture")]
        public byte[] ProfilePicture { get; set; }

        public int UsernameChangeLimit { get; set; } = 10;
    }
}
