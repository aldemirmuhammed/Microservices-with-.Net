namespace User.Management.API.Dtos.ChangePassword
{
    public class SetPasswordDto : AccountBaseDto
    {
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }


    }
}
