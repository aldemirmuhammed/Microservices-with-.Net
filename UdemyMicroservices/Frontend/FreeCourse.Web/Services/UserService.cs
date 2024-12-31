using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Account;
using FreeCourse.Web.Models.Account.User;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class UserService : IUserService
    {

        private readonly HttpClient _httpClient;
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserViewModel> GetUser()
        {
            var response =  await _httpClient.GetAsync("/api/User/GetUser");
            if (!response.IsSuccessStatusCode)
                return null;

            var role = await response.Content.ReadFromJsonAsync<UserViewModel>();
            return role;
        }

    }
}
