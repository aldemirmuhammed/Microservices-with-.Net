using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Web.Handler
{
    public class ResourceOwnerPasswordTokenHandler:DelegatingHandler
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityService _ıdentityService;
        private readonly ILogger<ResourceOwnerPasswordTokenHandler> _logger;

        public ResourceOwnerPasswordTokenHandler(IHttpContextAccessor httpContextAccessor, IIdentityService ıdentityService, ILogger<ResourceOwnerPasswordTokenHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _ıdentityService = ıdentityService;
            _logger = logger;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response=await base.SendAsync(request, cancellationToken);
            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var tokenResponse=await _ıdentityService.GetAccessTokenByRefreshToken();    

                if(tokenResponse!= null)
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                        tokenResponse.AccessToken);

                    response = await base.SendAsync(request, cancellationToken);

                }

            }

            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // hata fırlatılacak
                // Middleware tarafından karşılanacak

                throw new UnAuthorizeException();
            }
            return response;

        }
    }
}
