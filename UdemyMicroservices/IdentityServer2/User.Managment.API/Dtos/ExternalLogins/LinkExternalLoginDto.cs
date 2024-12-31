namespace User.Management.API.Dtos.ExternalLogins
{
    public class LinkExternalLoginDto : AccountBaseDto
    {
        public string LoginProvider { get; set; }
    }
}
