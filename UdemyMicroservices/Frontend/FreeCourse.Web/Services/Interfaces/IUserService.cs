using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Account;
using FreeCourse.Web.Models.Account.User;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IUserService
    {

        Task<UserViewModel> GetUser();
    }
}
