using FreeCourse.Web.Models.FakePayment;
using FreeCourse.Web.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        public PaymentService(HttpClient httpClient, IUserService userService)
        {
            _httpClient = httpClient;
            _userService = userService;
        }

        public async Task<bool> ReceivePayment(PaymentInfoInput paymentInfoInput)
        {
            var user = _userService.GetUser();
            paymentInfoInput.Order.BuyerEmail = user.Result.Email;
            var response = await _httpClient.PostAsJsonAsync<PaymentInfoInput>("fakepayments", paymentInfoInput);

            return response.IsSuccessStatusCode;
        }
    }
}
