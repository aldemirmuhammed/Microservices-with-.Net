using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Account.Admin.RoleManager;
using FreeCourse.Web.Models.Notifications;
using System.ComponentModel;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services
{
    public class RoleManagerService : IRoleManagerService
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceApiSettings _serviceApiSettings;
        private readonly ISharedIdentityService _sharedIdentityService;

        public RoleManagerService(HttpClient httpClient, IOptions<ServiceApiSettings> serviceApiSettings, ISharedIdentityService sharedIdentityService)
        {
            _httpClient = httpClient;
            _serviceApiSettings = serviceApiSettings.Value;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<bool> HasPermissionToRole()
        {
            var userId = _sharedIdentityService.GetUserId;
            if (userId == null)
                return false;
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Admin/HasPermissionToRole/{userId}");
            if (!response.IsSuccessStatusCode)
                return false;

            var role = await response.Content.ReadFromJsonAsync<Response<bool>>();
            if (role == null)
                return false;



            return role.Data;
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            if (roleName == null)
                return false;
            var response = await _httpClient.PostAsJsonAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Admin/CreateRoleAsync/{roleName}", roleName);
            if (!response.IsSuccessStatusCode)
                return false;
            var role = await response.Content.ReadFromJsonAsync<Response<bool>>();

            return role.Data;
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            if (roleId == null)
                return false;
            var response = await _httpClient.DeleteAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Admin/DeleteRoleAsync/{roleId}");
            if (!response.IsSuccessStatusCode)
                return false;
            var role = await response.Content.ReadFromJsonAsync<Response<bool>>();

            return role.Data;
        }

        public async Task<List<IdentityRole>> GetIdentityRoles()
        {

            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Admin/GetIdentityRoles");
            if (!response.IsSuccessStatusCode)
                return null;
            var role = await response.Content.ReadFromJsonAsync<Response<List<IdentityRole>>>();

            return role.Data;
        }

        public async Task<UserRolesViewModel> GetUserRolesByUserId(string userId)
        {
            if (userId == null)
                return null;
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Admin/GetUserRolesByUserId/{userId}");
            if (!response.IsSuccessStatusCode)
                return null;
            var role = await response.Content.ReadFromJsonAsync<Response<UserRolesViewModel>>();

            return role.Data;
        }

        public async Task<List<UserRolesViewModel>> GetAllUserRoles()
        {

            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Admin/GetAllUserRoles");
            if (!response.IsSuccessStatusCode)
                return null;
            var role = await response.Content.ReadFromJsonAsync<Response<List<UserRolesViewModel>>>();

            return role.Data;
        }


        public async Task<List<ManageUserRolesViewModel>> ManageUserRoles(string userId)
        {
            if (userId == null)
                return null;
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Admin/ManageUserRoles/{userId}");
            if (!response.IsSuccessStatusCode)
                return null;
            var role = await response.Content.ReadFromJsonAsync<Response<List<ManageUserRolesViewModel>>>();

            return role.Data;
        }

        public async Task<List<ManageUserRolesViewModel>> UpdateUserRoles(List<ManageUserRolesViewModel> model, string userId)
        {
            if (userId == null)
                return null;
            var response = await _httpClient.GetAsync($"{_serviceApiSettings.IdentityBaseUri}/api/Admin/UpdateUserRoles/{userId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var role = await response.Content.ReadFromJsonAsync<Response<List<ManageUserRolesViewModel>>>();


            return role.Data;
        }

    }
}
