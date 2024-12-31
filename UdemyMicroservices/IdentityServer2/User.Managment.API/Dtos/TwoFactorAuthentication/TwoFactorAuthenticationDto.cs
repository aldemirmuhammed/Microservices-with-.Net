namespace User.Management.API.Dtos.TwoFactorAuthentication
{
    public class TwoFactorAuthenticationDto : AccountBaseDto
    {

        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }
    }
}
