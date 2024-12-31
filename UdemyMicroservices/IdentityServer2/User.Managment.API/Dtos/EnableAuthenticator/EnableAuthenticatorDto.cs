namespace User.Management.API.Dtos.EnableAuthenticator
{
    public class EnableAuthenticatorDto : AccountBaseDto
    {

        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        public string Code { get; set; }

        public string[] RecoveryCodes { get; set; }

    }
}
