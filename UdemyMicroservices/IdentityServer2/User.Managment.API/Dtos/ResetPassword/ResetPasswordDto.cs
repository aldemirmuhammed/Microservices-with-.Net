using System.ComponentModel.DataAnnotations;

namespace User.Management.API.Dtos.ResetPassword
{
    public class ResetPasswordDto : AccountBaseDto
    {
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        public string? Code { get; set; }
    }
}
