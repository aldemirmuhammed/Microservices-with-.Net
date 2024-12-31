namespace User.Management.API.Dtos.SendEmailConfirmation
{
    public class SendEmailConfirmationDto : AccountBaseDto
    {
        public string Email { get; set; }

        public string Code { get; set; } = null;

    }
}
