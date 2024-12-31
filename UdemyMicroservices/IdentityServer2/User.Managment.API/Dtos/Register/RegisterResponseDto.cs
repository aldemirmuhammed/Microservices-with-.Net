using User.Management.API.Models;

namespace User.Management.API.Dtos.Register
{
    public class RegisterResponseDto
    {
        public string Token { get; set; } = null!;
        public bool RequireConfirmedAccount { get; set; } = false;
        public ApplicationUser User { get; set; } = null!;

    }


}
