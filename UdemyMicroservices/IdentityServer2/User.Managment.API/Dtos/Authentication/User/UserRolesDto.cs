namespace User.Management.API.Dtos.Authentication.User
{
    public class UserRolesDto
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }
}
