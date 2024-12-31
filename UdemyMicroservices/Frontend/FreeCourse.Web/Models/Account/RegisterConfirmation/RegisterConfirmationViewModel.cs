using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Account.RegisterConfirmation
{
    public class RegisterConfirmationViewModel : AccountBaseViewModel
    {
        [Required]
        public string Email { get; set; }


        public string Code { get; set; }

    }
}
