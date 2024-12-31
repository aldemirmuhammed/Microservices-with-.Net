using System.ComponentModel.DataAnnotations;

namespace User.Management.API.Dtos.Register
{
    public class RegisterConfirmationDto : AccountBaseDto
    {
        [Required]
        public string Email { get; set; }

        
        public string Code { get; set; }
    }
}
