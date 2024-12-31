using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public EmailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<Response<EmailMessage>> SendEmailAsync(EmailMessage message)
        {
            var service = _configuration.GetSection("Services:Email");
            var response = await _httpClient.PostAsJsonAsync($"{service}/api/Email/SendMail", message);
            if (!response.IsSuccessStatusCode)
                return Response<EmailMessage>.Fail("User email confirmation token couldnt generated error.", 404);

            var result = await response.Content.ReadFromJsonAsync<Response<EmailMessage>>();
            if (result.Data == null || (result.Erros != null && result.Erros.Any()))
                return Response<EmailMessage>.Fail(result.Erros, 404);


            return Response<EmailMessage>.Success(message, 200);
        }


    }
}
