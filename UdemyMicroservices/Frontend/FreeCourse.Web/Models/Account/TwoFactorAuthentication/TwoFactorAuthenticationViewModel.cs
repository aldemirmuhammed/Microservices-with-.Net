using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Models.Account.TwoFactorAuthentication
{
    public class TwoFactorAuthenticationViewModel : AccountBaseViewModel
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }
    }
}
