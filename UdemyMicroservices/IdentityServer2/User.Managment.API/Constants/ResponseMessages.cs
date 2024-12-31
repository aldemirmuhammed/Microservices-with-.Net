namespace User.Management.API.Constants
{
    public static class ResponseMessages
    {
        public static string GetEmailSuccessMessage(string emailAddress) =>  $"Email sent successfully to {emailAddress}";
        

    }
    public static class EnableAuthenticator
    {
        public const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    }
}
