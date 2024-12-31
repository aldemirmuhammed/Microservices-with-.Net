namespace User.Management.API.Dtos.GenerateRecoveryCodes
{
    public class GenerateRecoveryCodesDto : AccountBaseDto
    {
        public string[] RecoveryCodes { get; set; }

    }
}
