namespace FreeCourse.Web.Models.Account.Register
{
    public class RegisterResponseViewModel
    {
        public string Token { get; set; } = null!;
        public bool RequireConfirmedAccount { get; set; } = false;
        public ApplicationUser User { get; set; } = null!;

    }
  

}
