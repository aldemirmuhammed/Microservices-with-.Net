using System.Collections.Generic;
using System;

namespace FreeCourse.Web.Models.Orders
{
    public class OrderViewModel
    {

        public int Id { get; set; }

        public DateTime CreatedDate { get;  set; }

       // public AddressDto Address { get; private set; }

        public string BuyerId { get;  set; }

        public List<OrderItemViewModel> OrderItems { get; set; }

    }
}
