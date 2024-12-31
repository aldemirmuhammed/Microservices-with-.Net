using System.ComponentModel.DataAnnotations;

namespace User.Management.API.Dtos.Register
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }


        [Required(ErrorMessage = "Phone is required")]
        public string? Phone { get; set; }

        public bool RequireConfirmedAccount { get; set; }

        public List<string>? Roles { get; set; }

    }
}
