using FreeCourse.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using FreeCourse.Shared.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Writers;
using System;
using System.Threading.Tasks;

namespace FreeCourse.Services.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentsController : CustomBaseController
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        //private readonly UserManager<IdentityUser> _userManager;

        public FakePaymentsController(ISendEndpointProvider sendEndpointProvider/*, UserManager<IdentityUser> manager*/)
        {
            _sendEndpointProvider = sendEndpointProvider;
            //_userManager = manager;
        }

        [HttpPost]
        public async Task<IActionResult> ReceivePayment(PaymentDto paymentDto)
        {

            //var usr = await _userManager.GetUserAsync(HttpContext.User);
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-service"));


            var createOrderMessageCommand = new CreateOrderMessageCommand();

            createOrderMessageCommand.BuyerId = paymentDto.Order.BuyerId;
            createOrderMessageCommand.BuyerEmail = paymentDto.Order.BuyerEmail;
            createOrderMessageCommand.Province = paymentDto.Order.Address.Province;
            createOrderMessageCommand.District = paymentDto.Order.Address.District;
            createOrderMessageCommand.Street = paymentDto.Order.Address.Street;
            createOrderMessageCommand.Line = paymentDto.Order.Address.Line;
            createOrderMessageCommand.ZipCode = paymentDto.Order.Address.ZipCode;

            createOrderMessageCommand.CardName = paymentDto.CardName;
            createOrderMessageCommand.CardNumber = paymentDto.CardNumber;
            createOrderMessageCommand.Expiration = paymentDto.Expiration;
            createOrderMessageCommand.CVV = paymentDto.CVV;
            createOrderMessageCommand.TotalPrice = paymentDto.TotalPrice;


            paymentDto.Order.OrderItems.ForEach(x =>
            {
                createOrderMessageCommand.OrderItems.Add(new OrderItem
                {
                    PictureUrl = x.PictureUrl,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                });
            });

            await sendEndpoint.Send<CreateOrderMessageCommand>(createOrderMessageCommand);

            return CreateActionResultInstance(FreeCourse.Shared.Dtos.Response<NoContent>.Success(200));

        }


    }
}
