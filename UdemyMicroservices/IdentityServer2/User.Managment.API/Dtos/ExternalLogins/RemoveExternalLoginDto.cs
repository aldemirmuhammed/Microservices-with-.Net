namespace User.Management.API.Dtos.ExternalLogins
{
    public class RemoveExternalLoginDto : LinkExternalLoginDto
    {
       
        public string ProviderKey { get; set; }
    }
}
