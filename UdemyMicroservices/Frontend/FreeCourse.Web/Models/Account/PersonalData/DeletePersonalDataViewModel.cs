using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Account.PersonalData
{
    public class DeletePersonalDataViewModel : AccountBaseViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
