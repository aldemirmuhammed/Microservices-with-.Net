using User.Management.API.Models;

namespace User.Management.API.Dtos.Login
{
    public class LoginOtpResponseDto
    {
        public string Token { get; set; } = null!;
        public bool IsTwoFactorEnable { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
