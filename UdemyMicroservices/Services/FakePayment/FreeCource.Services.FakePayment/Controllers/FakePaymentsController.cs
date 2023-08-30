﻿using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCource.Services.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentsController : CustomBaseController
    {


        [HttpGet]
        public IActionResult ReceivePayment() 
        {
            return CreateActionResultInstance(Response<NoContent>.Success(200));
        
        }
    }
}