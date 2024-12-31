namespace User.Management.API.Dtos.ChangeEmail
{
    public class ChangeEmailDto : AccountBaseDto
    {
        public string NewEmail { get; set; }
        public string Token { get; set; } = null;
    }
}
