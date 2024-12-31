using System.ComponentModel.DataAnnotations;

namespace User.Management.API.Dtos.ChangePassword
{
    public class ChangePasswordDto : AccountBaseDto
    {

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }

    }
}
