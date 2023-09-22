using FreeCource.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FreeCource.Services.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentsController : CustomBaseController
    {


        [HttpPost]
        public async Task<IActionResult> ReceivePayment(PaymentDto paymentDto) 
        {
            return CreateActionResultInstance(Response<NoContent>.Success(200));
        
        }

    
    }
}
