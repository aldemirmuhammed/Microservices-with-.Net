namespace User.Management.API.Dtos.ForgotPassword
{
    public class ForgotPasswordDto : AccountBaseDto
    {
        public string Email { get; set; }

        public string Code { get; set; } = null;

    }
}
